import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { Photo } from '../_models/Photo';
import { PaginationResult } from '../_models/pagination';
import { UserParams } from '../_models/UserParams';
import { of } from 'rxjs';
import { setPaginatedResponse, setPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  http = inject(HttpClient);
  BaseUrl = environment.apiUrl;
  paginationResult = signal<PaginationResult<Member[]> | null>(null);
  memberCache = new Map();
  getMembers(userParams: UserParams) {
    const respone = this.memberCache.get(Object.values(userParams).join('-'));
    if (respone) return setPaginatedResponse(respone, this.paginationResult);
    let params = setPaginationHeaders(
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
          setPaginatedResponse(response, this.paginationResult);
          this.memberCache.set(Object.values(userParams).join('-'), response);
        },
      });
  }

  clearCache() {
    this.memberCache.clear();
    this.paginationResult.set(null);
  }
  getMember(username: string) {
    const cachedResponses = Array.from(this.memberCache.values());
    const allMembers: Member[] = cachedResponses.flatMap(
      (resp) => resp.body ?? []
    );
    const member: Member | undefined = allMembers.find(
      (m) => m.username === username
    );
    if (member) return of(member);

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
