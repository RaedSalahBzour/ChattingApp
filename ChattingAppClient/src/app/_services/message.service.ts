import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PaginationResult } from '../_models/pagination';
import { Message } from '../_models/message';
import { setPaginatedResponse, setPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  paginatedResult = signal<PaginationResult<Message[]> | null>(null);
  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params = setPaginationHeaders(pageNumber, pageSize);
    params = params.append('container', container);
    return this.http
      .get<Message[]>(this.baseUrl + 'message', { observe: 'response', params })
      .subscribe({
        next: (response) =>
          setPaginatedResponse(response, this.paginatedResult),
      });
  }
  getMessageThread(username: string) {
    return this.http.get<Message[]>(
      this.baseUrl + 'message/thread/' + username
    );
  }
  sendMessage(username: string, content: string) {
    return this.http.post<Message>(this.baseUrl + 'message', {
      recipientUsername: username,
      content,
    });
  }
  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'message/' + id);
  }
}
