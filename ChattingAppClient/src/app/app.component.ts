import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { NavbarComponent } from './navbar/navbar.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [NavbarComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  users: any;
  constructor(private httpClient: HttpClient) {}
  ngOnInit(): void {
    this.httpClient.get('https://localhost:5001/api/user').subscribe({
      next: (response) => {
        this.users = response;
      },
      error: (error) => console.log(error),
      complete: () => {
        console.log('request has completed');
      },
    });
  }
}
