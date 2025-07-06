import { Component, inject, OnInit } from '@angular/core';
import { MemberService } from '../../../_services/member.service';
import { RouterModule } from '@angular/router';
import { MemberCardComponent } from '../member-card/member-card.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { AccountService } from '../../../_services/account.service';
import { UserParams } from '../../../_models/UserParams';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [RouterModule, MemberCardComponent, PaginationModule, FormsModule],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css',
})
export class MemberListComponent implements OnInit {
  memberService = inject(MemberService);
  private accountService = inject(AccountService);
  userParams = new UserParams(this.accountService.currentUser());
  genderList = [
    { value: 'female', display: 'female' },
    { value: 'male', display: 'male' },
  ];
  OrderByList = [
    { value: 'createdAt', display: 'created' },
    { value: 'lastActive', display: 'lastActive' },
  ];
  ngOnInit(): void {
    this.loadMembers();
  }
  loadMembers() {
    this.memberService.getMembers(this.userParams);
  }
  resetFilters() {
    this.userParams = new UserParams(this.accountService.currentUser());
    this.loadMembers();
  }
  pageChanged(event: any) {
    if (this.userParams.pageNumber !== event.page) {
      this.userParams.pageNumber = event.page;
      this.loadMembers();
    }
  }
}
