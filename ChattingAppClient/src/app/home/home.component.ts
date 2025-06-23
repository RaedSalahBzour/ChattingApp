import { Component, inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AccountService } from '../_services/account.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit {
  users: any;
  constructor(private httpClient: HttpClient) {}
  accountService = inject(AccountService);
  ngOnInit(): void {
    this.getUsers();
  }

  getUsers() {
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
