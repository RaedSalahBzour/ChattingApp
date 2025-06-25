import { Component, inject, OnInit } from '@angular/core';
import { MemberService } from '../../../_services/member.service';
import { Member } from '../../../_models/member';
import { RouterModule } from '@angular/router';
import { MemberDetailComponent } from '../member-detail/member-detail.component';
import { MemberCardComponent } from '../member-card/member-card.component';

@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [RouterModule, MemberCardComponent],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css',
})
export class MemberListComponent implements OnInit {
  memberService = inject(MemberService);
  members: Member[] = [];
  ngOnInit(): void {
    this.loadMembers();
  }
  loadMembers() {
    this.memberService
      .getMembers()
      .subscribe((members) => (this.members = members));
  }
}
