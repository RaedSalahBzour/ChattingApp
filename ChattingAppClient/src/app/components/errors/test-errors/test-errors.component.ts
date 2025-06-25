import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-test-errors',
  standalone: true,
  imports: [],
  templateUrl: './test-errors.component.html',
  styleUrl: './test-errors.component.css',
})
export class TestErrorsComponent {
  http = inject(HttpClient);
  BaseUrl = environment.apiUrl;
  validationErrors: string[] = [];
  get400error() {
    this.http.get(this.BaseUrl + 'Buggy/bad-request').subscribe({
      next: (response) => console.log(response),
      error: (error) => console.error(error),
    });
  }
  get401error() {
    this.http.get(this.BaseUrl + 'Buggy/auth').subscribe({
      next: (response) => console.log(response),
      error: (error) => console.error(error),
    });
  }
  get404error() {
    this.http.get(this.BaseUrl + 'Buggy/not-found').subscribe({
      next: (response) => console.log(response),
      error: (error) => console.error(error),
    });
  }
  get500error() {
    this.http.get(this.BaseUrl + 'Buggy/server-error').subscribe({
      next: (response) => console.log(response),
      error: (error) => console.error(error),
    });
  }
  get400Validationerror() {
    this.http.post(this.BaseUrl + 'account/register', {}).subscribe({
      next: (response) => console.log(response),
      error: (error) => {
        console.error(error);
        this.validationErrors = error;
      },
    });
  }
}
