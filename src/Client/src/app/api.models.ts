export interface AuthResponse {
  userId: string;
  email: string;
  accessToken: string;
  expiresAtUtc: string;
}

export interface CategoryModel {
  id: string;
  name: string;
}

export interface TaskModel {
  id: string;
  title: string;
  isCompleted: boolean;
  createdAtUtc: string;
  dueDateUtc: string | null;
  categoryId: string | null;
  categoryName: string | null;
}

export interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  start: number;
  end: number;
  count: number;
  hasNextPage: boolean;
}

export interface TaskDraft {
  title: string;
  dueDate: string;
  categoryId: string;
}

export interface TaskEditDraft extends TaskDraft {
  isCompleted: boolean;
}
