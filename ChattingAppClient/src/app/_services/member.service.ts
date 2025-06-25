import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  http = inject(HttpClient);
  BaseUrl = environment.apiUrl;
  getMembers() {
    return this.http.get<Member[]>(this.BaseUrl + 'user');
  }
  getMember(username: string) {
    return this.http.get<Member>(this.BaseUrl + 'user/' + username);
  }
}
