import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { of, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  http = inject(HttpClient);
  BaseUrl = environment.apiUrl;
  members = signal<Member[]>([]);
  getMembers() {
    return this.http
      .get<Member[]>(this.BaseUrl + 'user')
      .subscribe({ next: (members) => this.members.set(members) });
  }
  getMember(username: string) {
    const member = this.members().find((m) => m.username === username);
    if (member !== undefined) return of(member);
    return this.http.get<Member>(this.BaseUrl + 'user/' + username);
  }
  updateMember(member: Member) {
    return this.http
      .put(this.BaseUrl + 'user', member)
      .pipe(
        tap(() =>
          this.members.update((members) =>
            members.map((m) => (m.username === member.username ? member : m))
          )
        )
      );
  }
}
