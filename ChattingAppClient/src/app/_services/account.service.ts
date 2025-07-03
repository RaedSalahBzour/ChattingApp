import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { User } from '../_models/user';
import { tap } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private http = inject(HttpClient);
  BaseUrl = environment.apiUrl;
  currentUser = signal<User | null>(null);
  login(model: any) {
    return this.http.post<User>(this.BaseUrl + 'account/login', model).pipe(
      tap((user) => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }
  logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }
  register(user: any) {
    return this.http.post<User>(this.BaseUrl + 'account/register', user).pipe(
      tap((user) => {
        if (user) {
          this.setCurrentUser(user);
        }
        return user;
      })
    );
  }
  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
  }
}
