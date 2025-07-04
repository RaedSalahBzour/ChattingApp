import { Component, inject, input, OnInit } from '@angular/core';
import { Member } from '../../../_models/member';
import { MemberService } from '../../../_services/member.service';
import { ActivatedRoute } from '@angular/router';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [TabsModule, GalleryModule, TimeagoModule, DatePipe],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css',
})
export class MemberDetailComponent implements OnInit {
  memberService = inject(MemberService);
  route = inject(ActivatedRoute);
  member?: Member;
  images: GalleryItem[] = [];
  ngOnInit(): void {
    this.loadMember();
  }
  loadMember() {
    const username = this.route.snapshot.paramMap.get('username');
    if (!username) return;
    this.memberService.getMember(username).subscribe({
      next: (member) => {
        this.member = member;
        member.photos.map((p) => {
          this.images.push(new ImageItem({ src: p.url, thumb: p.url }));
        });
      },
    });
  }
}
