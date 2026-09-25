import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthPage } from './auth-page/auth-page';
import { AuthResponse } from './api.models';
import { WorkspacePage } from './workspace-page/workspace-page';

@Component({
  selector: 'app-root',
  imports: [AuthPage, RouterOutlet, WorkspacePage],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  token = localStorage.getItem('softplus_access_token') ?? '';
  currentEmail = localStorage.getItem('softplus_email') ?? '';
  authErrorMessage = '';

  get isAuthenticated(): boolean {
    return this.token.length > 0;
  }

  handleAuthenticated(response: AuthResponse): void {
    this.token = response.accessToken;
    this.currentEmail = response.email;
    this.authErrorMessage = '';
    localStorage.setItem('softplus_access_token', response.accessToken);
    localStorage.setItem('softplus_email', response.email);
  }

  handleLogout(message?: string): void {
    this.token = '';
    this.currentEmail = '';
    this.authErrorMessage = message ?? '';
    localStorage.removeItem('softplus_access_token');
    localStorage.removeItem('softplus_email');
  }
}
