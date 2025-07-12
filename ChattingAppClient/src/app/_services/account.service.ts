import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { User } from '../_models/user';
import { tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LikeService } from './like.service';
import { MemberService } from './member.service';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private http = inject(HttpClient);
  private likeService = inject(LikeService);
  private memberService = inject(MemberService);
  BaseUrl = environment.apiUrl;
  currentUser = signal<User | null>(null);
  roles = computed(() => {
    var user = this.currentUser();
    if (user && user.token) {
      const role = JSON.parse(atob(user.token.split('.')[1])).role;
      return Array.isArray(role) ? role : [role];
    }
    return [];
  });
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
    this.memberService.clearCache();
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
    this.likeService.getLikeIds();
  }
}
