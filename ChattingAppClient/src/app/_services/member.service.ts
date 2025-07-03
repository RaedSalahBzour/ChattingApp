import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { Photo } from '../_models/Photo';
import { PaginationResult } from '../_models/pagination';
import { UserParams } from '../_models/UserParams';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  http = inject(HttpClient);
  BaseUrl = environment.apiUrl;
  paginationResult = signal<PaginationResult<Member[]> | null>(null);
  getMembers(userParams: UserParams) {
    let params = this.setPaginationHeaders(
      userParams.pageNumber,
      userParams.pageSize
    );
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.OrderBy);

    return this.http
      .get<Member[]>(this.BaseUrl + 'user', { observe: 'response', params })
      .subscribe({
        next: (response) => {
          this.paginationResult.set({
            items: response.body as Member[],
            pagination: JSON.parse(
              response.headers.get('Pagination') as string
            ),
          });
        },
      });
  }
  private setPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();
    if (pageNumber && pageSize) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }
    return params;
  }
  getMember(username: string) {
    // const member = this.members().find((m) => m.username === username);
    // if (member !== undefined) return of(member);
    return this.http.get<Member>(this.BaseUrl + 'user/' + username);
  }
  updateMember(member: Member) {
    return this.http
      .put(this.BaseUrl + 'user', member)
      .pipe
      // tap(() =>
      //   this.members.update((members) =>
      //     members.map((m) => (m.username === member.username ? member : m))
      //   )
      // )
      ();
  }
  setMainPhoto(photo: Photo) {
    return this.http
      .put(this.BaseUrl + 'user/set-main-photo/' + photo.id, {})
      .pipe
      // tap(() => {
      //   this.members.update((members) =>
      //     members.map((m) => {
      //       if (m.photos.includes(photo)) {
      //         m.photoUrl = photo.url;
      //       }
      //       return m;
      //     })
      //   );
      // })
      ();
  }
  deletePhoto(photo: Photo) {
    return this.http
      .delete(this.BaseUrl + 'user/delete-photo/' + photo.id)
      .pipe
      // tap(() => {
      //   this.members.update((members) =>
      //     members.map((m) => {
      //       if (m.photos.includes(photo)) {
      //         m.photos = m.photos.filter((p) => p.id !== photo.id);
      //       }
      //       return m;
      //     })
      //   );
      // })
      ();
  }
}
