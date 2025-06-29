import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { of, tap } from 'rxjs';
import { Photo } from '../_models/Photo';

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
  setMainPhoto(photo: Photo) {
    return this.http
      .put(this.BaseUrl + 'user/set-main-photo/' + photo.id, {})
      .pipe(
        tap(() => {
          this.members.update((members) =>
            members.map((m) => {
              if (m.photos.includes(photo)) {
                m.photoUrl = photo.url;
              }
              return m;
            })
          );
        })
      );
  }
  deletePhoto(photo: Photo) {
    return this.http
      .delete(this.BaseUrl + 'user/delete-photo/' + photo.id)
      .pipe(
        tap(() => {
          this.members.update((members) =>
            members.map((m) => {
              if (m.photos.includes(photo)) {
                m.photos = m.photos.filter((p) => p.id !== photo.id);
              }
              return m;
            })
          );
        })
      );
  }
}
