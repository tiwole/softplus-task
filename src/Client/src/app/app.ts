import { CommonModule } from '@angular/common';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterOutlet } from '@angular/router';

interface AuthResponse {
  userId: string;
  email: string;
  accessToken: string;
  expiresAtUtc: string;
}

interface CategoryModel {
  id: string;
  name: string;
}

interface TaskModel {
  id: string;
  title: string;
  isCompleted: boolean;
  createdAtUtc: string;
  dueDateUtc: string | null;
  categoryId: string | null;
  categoryName: string | null;
}

interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  start: number;
  end: number;
  count: number;
  hasNextPage: boolean;
}

interface TaskDraft {
  title: string;
  dueDate: string;
  categoryId: string;
}

interface TaskEditDraft extends TaskDraft {
  isCompleted: boolean;
}

@Component({
  selector: 'app-root',
  imports: [CommonModule, FormsModule, RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly changeDetector = inject(ChangeDetectorRef);
  private readonly apiUrl = 'http://localhost:5000/api';

  authMode: 'login' | 'register' = 'login';
  authForm = { email: '', password: '' };
  token = localStorage.getItem('softplus_access_token') ?? '';
  currentEmail = localStorage.getItem('softplus_email') ?? '';

  categories: CategoryModel[] = [];
  tasks: TaskModel[] = [];
  taskPage: PaginatedResponse<TaskModel> | null = null;

  taskDraft: TaskDraft = this.emptyTaskDraft();
  editTaskId: string | null = null;
  editDraft: TaskEditDraft = this.emptyEditDraft();

  newCategoryName = '';
  editingCategoryId: string | null = null;
  editingCategoryName = '';

  search = '';
  selectedCategoryId = '';
  start = 0;
  limit = 10;

  loading = false;
  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    if (this.isAuthenticated) {
      this.loadWorkspace();
    }
  }

  get isAuthenticated(): boolean {
    return this.token.length > 0;
  }

  get totalPages(): number {
    if (!this.taskPage || this.limit <= 0) {
      return 1;
    }

    return Math.max(1, Math.ceil(this.taskPage.totalCount / this.limit));
  }

  get currentPage(): number {
    return Math.floor(this.start / this.limit) + 1;
  }

  submitAuth(): void {
    this.clearMessages();
    this.loading = true;

    const endpoint = this.authMode === 'login' ? 'login' : 'register';
    this.http.post<AuthResponse>(`${this.apiUrl}/auth/${endpoint}`, this.authForm).subscribe({
      next: response => {
        this.token = response.accessToken;
        this.currentEmail = response.email;
        localStorage.setItem('softplus_access_token', response.accessToken);
        localStorage.setItem('softplus_email', response.email);
        this.authForm = { email: '', password: '' };
        this.loading = false;
        this.loadWorkspace();
        this.refreshView();
      },
      error: error => this.handleError(error)
    });
  }

  logout(): void {
    this.token = '';
    this.currentEmail = '';
    this.categories = [];
    this.tasks = [];
    this.taskPage = null;
    localStorage.removeItem('softplus_access_token');
    localStorage.removeItem('softplus_email');
    this.refreshView();
  }

  loadWorkspace(): void {
    this.loadCategories();
    this.loadTasks();
  }

  loadCategories(): void {
    this.http.get<CategoryModel[]>(`${this.apiUrl}/categories`, { headers: this.authHeaders() }).subscribe({
      next: categories => {
        this.categories = categories;
        this.refreshView();
      },
      error: error => this.handleError(error)
    });
  }

  loadTasks(resetStart = false): void {
    if (resetStart) {
      this.start = 0;
    }

    this.clearMessages();
    this.loading = true;

    let params = new HttpParams()
      .set('start', this.start)
      .set('limit', this.limit);

    if (this.search.trim()) {
      params = params.set('search', this.search.trim());
    }

    if (this.selectedCategoryId) {
      params = params.set('categoryId', this.selectedCategoryId);
    }

    this.http.get<PaginatedResponse<TaskModel>>(`${this.apiUrl}/tasks`, {
      headers: this.authHeaders(),
      params
    }).subscribe({
      next: response => {
        this.taskPage = response;
        this.tasks = response.data;
        this.loading = false;
        this.refreshView();
      },
      error: error => this.handleError(error)
    });
  }

  createTask(): void {
    if (!this.taskDraft.title.trim()) {
      this.errorMessage = 'Task title is required.';
      return;
    }

    this.clearMessages();

    this.http.post<TaskModel>(`${this.apiUrl}/tasks`, this.toTaskRequest(this.taskDraft), {
      headers: this.authHeaders()
    }).subscribe({
      next: () => {
        this.taskDraft = this.emptyTaskDraft();
        this.successMessage = 'Task created.';
        this.loadTasks(true);
        this.refreshView();
      },
      error: error => this.handleError(error)
    });
  }

  startTaskEdit(task: TaskModel): void {
    this.editTaskId = task.id;
    this.editDraft = {
      title: task.title,
      isCompleted: task.isCompleted,
      dueDate: this.toDateInput(task.dueDateUtc),
      categoryId: task.categoryId ?? ''
    };
  }

  cancelTaskEdit(): void {
    this.editTaskId = null;
    this.editDraft = this.emptyEditDraft();
  }

  updateTask(task: TaskModel): void {
    if (!this.editDraft.title.trim()) {
      this.errorMessage = 'Task title is required.';
      return;
    }

    this.clearMessages();

    this.http.put<TaskModel>(`${this.apiUrl}/tasks/${task.id}`, {
      ...this.toTaskRequest(this.editDraft),
      isCompleted: this.editDraft.isCompleted
    }, { headers: this.authHeaders() }).subscribe({
      next: () => {
        this.cancelTaskEdit();
        this.successMessage = 'Task updated.';
        this.loadTasks();
        this.refreshView();
      },
      error: error => this.handleError(error)
    });
  }

  toggleTask(task: TaskModel): void {
    const request = {
      title: task.title,
      isCompleted: !task.isCompleted,
      dueDateUtc: task.dueDateUtc,
      categoryId: task.categoryId
    };

    this.http.put<TaskModel>(`${this.apiUrl}/tasks/${task.id}`, request, {
      headers: this.authHeaders()
    }).subscribe({
      next: () => this.loadTasks(),
      error: error => this.handleError(error)
    });
  }

  deleteTask(task: TaskModel): void {
    this.http.delete<void>(`${this.apiUrl}/tasks/${task.id}`, {
      headers: this.authHeaders()
    }).subscribe({
      next: () => {
        this.successMessage = 'Task deleted.';
        this.loadTasks();
        this.refreshView();
      },
      error: error => this.handleError(error)
    });
  }

  createCategory(): void {
    if (!this.newCategoryName.trim()) {
      this.errorMessage = 'Category name is required.';
      return;
    }

    this.http.post<CategoryModel>(`${this.apiUrl}/categories`, { name: this.newCategoryName.trim() }, {
      headers: this.authHeaders()
    }).subscribe({
      next: () => {
        this.newCategoryName = '';
        this.successMessage = 'Category added.';
        this.loadCategories();
        this.refreshView();
      },
      error: error => this.handleError(error)
    });
  }

  startCategoryEdit(category: CategoryModel): void {
    this.editingCategoryId = category.id;
    this.editingCategoryName = category.name;
  }

  cancelCategoryEdit(): void {
    this.editingCategoryId = null;
    this.editingCategoryName = '';
  }

  updateCategory(category: CategoryModel): void {
    if (!this.editingCategoryName.trim()) {
      this.errorMessage = 'Category name is required.';
      return;
    }

    this.http.put<CategoryModel>(`${this.apiUrl}/categories/${category.id}`, {
      name: this.editingCategoryName.trim()
    }, { headers: this.authHeaders() }).subscribe({
      next: () => {
        this.cancelCategoryEdit();
        this.successMessage = 'Category updated.';
        this.loadWorkspace();
        this.refreshView();
      },
      error: error => this.handleError(error)
    });
  }

  deleteCategory(category: CategoryModel): void {
    this.http.delete<void>(`${this.apiUrl}/categories/${category.id}`, {
      headers: this.authHeaders()
    }).subscribe({
      next: () => {
        if (this.selectedCategoryId === category.id) {
          this.selectedCategoryId = '';
        }
        this.successMessage = 'Category deleted.';
        this.loadWorkspace();
        this.refreshView();
      },
      error: error => this.handleError(error)
    });
  }

  getDueDateClass(dueDateUtc: string | Date): string {
    const dueDate = new Date(dueDateUtc);
    const now = new Date();

    const threeDaysFromNow = new Date();
    threeDaysFromNow.setDate(now.getDate() + 3);

    if (dueDate < now) {
      return 'text-red-500';
    }

    if (dueDate <= threeDaysFromNow) {
      return 'text-yellow-500';
    }

    return '';
  }

  applyFilters(): void {
    this.loadTasks(true);
  }

  clearFilters(): void {
    this.search = '';
    this.selectedCategoryId = '';
    this.loadTasks(true);
  }

  nextPage(): void {
    if (!this.taskPage?.hasNextPage) {
      return;
    }

    this.start += this.limit;
    this.loadTasks();
  }

  previousPage(): void {
    this.start = Math.max(0, this.start - this.limit);
    this.loadTasks();
  }

  changeLimit(value: string): void {
    this.limit = Number(value);
    this.loadTasks(true);
  }

  private authHeaders(): HttpHeaders {
    return new HttpHeaders({
      Authorization: `Bearer ${this.token}`
    });
  }

  private toTaskRequest(draft: TaskDraft): { title: string; dueDateUtc: string | null; categoryId: string | null } {
    return {
      title: draft.title.trim(),
      dueDateUtc: draft.dueDate ? new Date(`${draft.dueDate}T00:00:00.000Z`).toISOString() : null,
      categoryId: draft.categoryId || null
    };
  }

  private toDateInput(value: string | null): string {
    return value ? value.slice(0, 10) : '';
  }

  private emptyTaskDraft(): TaskDraft {
    return { title: '', dueDate: '', categoryId: '' };
  }

  private emptyEditDraft(): TaskEditDraft {
    return { title: '', isCompleted: false, dueDate: '', categoryId: '' };
  }

  private clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  private handleError(error: HttpErrorResponse): void {
    this.loading = false;

    if (error.status === 401) {
      this.logout();
      this.errorMessage = 'Session expired. Log in again.';
      this.refreshView();
      return;
    }

    const body = error.error as { errorMessage?: string } | null;
    this.errorMessage = body?.errorMessage ?? 'Request failed.';
    this.refreshView();
  }

  private refreshView(): void {
    this.changeDetector.markForCheck();
  }
}
