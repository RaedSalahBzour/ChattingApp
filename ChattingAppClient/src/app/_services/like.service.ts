import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class LikeService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  likeIds = signal<number[]>([]);

  toggleLike(targetId: number) {
    return this.http.post(`${this.baseUrl}like/${targetId}`, {});
  }
  getLikes(predicate: string) {
    return this.http.get(`${this.baseUrl}like?predicate=${predicate}`);
  }
  getLikeIds() {
    return this.http.get<number[]>(`${this.baseUrl}like/list`).subscribe({
      next: (likeIds) => this.likeIds.set(likeIds),
    });
  }
}
