import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private http = inject(HttpClient);
  BaseUrl = 'https://localhost:5001/api/';
  login(model: any) {
    return this.http.post(this.BaseUrl + 'account/login', model);
  }
}
