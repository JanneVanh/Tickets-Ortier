import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { User } from '../../shared/Models/User';

export interface AuthState {
  isAuthenticated: boolean;
  user?: User;
}

@Injectable({
  providedIn: 'root'
})
export class Account {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  currentUser = signal<User | null>(null);

  login(values: any) {
    let params = new HttpParams();
    params = params.append('useCookies', true);
    return this.http.post(this.baseUrl + 'api/login', values, {
      params,
      withCredentials: true
    }).pipe(
      tap(() => {
        // After successful login, get the full user data with roles
        this.getCurrentUser().subscribe();
      })
    );
  }

  register(values: any) {
    return this.http.post(this.baseUrl + 'account/register', values, {
      withCredentials: true
    });
  }

  logout() {
    return this.http.post(this.baseUrl + 'account/logout', {}, {
      withCredentials: true
    }).pipe(
      tap(() => {
        this.currentUser.set(null);
      })
    );
  }

  getAuthState() {
    return this.http.get<AuthState>(this.baseUrl + 'account/auth-status', {
      withCredentials: true
    });
  }

  getCurrentUser() {
    return this.http.get<User>(this.baseUrl + 'account/user', {
      withCredentials: true
    }).pipe(
      tap(user => {
        if (user) {
          this.currentUser.set(user);
        }
      })
    );
  }
}
