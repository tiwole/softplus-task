import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectorRef, Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthResponse } from '../api.models';

@Component({
  selector: 'app-auth-page',
  imports: [FormsModule],
  templateUrl: './auth-page.html'
})
export class AuthPage {
  private readonly http = inject(HttpClient);
  private readonly changeDetector = inject(ChangeDetectorRef);
  private readonly apiUrl = 'http://localhost:5000/api';

  @Output() authenticated = new EventEmitter<AuthResponse>();

  authMode: 'login' | 'register' = 'login';
  authForm = { email: '', password: '' };
  loading = false;
  errorMessage = '';

  @Input() set initialErrorMessage(value: string) {
    this.errorMessage = value;
  }

  submitAuth(): void {
    this.errorMessage = '';
    this.loading = true;

    const endpoint = this.authMode === 'login' ? 'login' : 'register';
    this.http.post<AuthResponse>(`${this.apiUrl}/auth/${endpoint}`, this.authForm).subscribe({
      next: response => {
        this.authForm = { email: '', password: '' };
        this.loading = false;
        this.authenticated.emit(response);
        this.refreshView();
      },
      error: error => this.handleError(error)
    });
  }

  private handleError(error: HttpErrorResponse): void {
    this.loading = false;

    const body = error.error as { errorMessage?: string } | null;
    this.errorMessage = body?.errorMessage ?? 'Request failed.';
    this.refreshView();
  }

  private refreshView(): void {
    this.changeDetector.markForCheck();
  }
}
