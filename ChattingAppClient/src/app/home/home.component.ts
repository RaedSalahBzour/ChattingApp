import { Component, OnInit } from '@angular/core';
import { RegisterComponent } from '../register/register.component';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RegisterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit {
  registerMode = false;
  users: any;
  constructor(private httpClient: HttpClient) {}
  ngOnInit(): void {
    this.getUsers();
  }
  registerToggle() {
    this.registerMode = !this.registerMode;
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
  handleCancelRegister(state: boolean) {
    this.registerMode = state;
  }
}
