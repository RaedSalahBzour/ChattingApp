import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLinkActive, RouterModule } from '@angular/router';
import { AccountService } from '../../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { TitleCasePipe } from '@angular/common';
import { MemberService } from '../../_services/member.service';
import { UserParams } from '../../_models/UserParams';
import { HasRoleDirective } from '../../_directives/has-role.directive';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    FormsModule,
    RouterModule,
    RouterLinkActive,
    TitleCasePipe,
    HasRoleDirective,
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent {
  accountService = inject(AccountService);
  router = inject(Router);
  toastrService = inject(ToastrService);
  model: any = {};
  login() {
    this.accountService.login(this.model).subscribe({
      next: () => {
        this.router.navigateByUrl('/home');
      },
      error: (error) => {
        console.log(error);
        this.toastrService.error(error.error);
      },
    });
  }
  logout() {
    this.accountService.logout();
  }
}
