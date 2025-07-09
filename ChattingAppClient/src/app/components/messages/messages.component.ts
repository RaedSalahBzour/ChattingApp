import { Component, inject, OnInit } from '@angular/core';
import { MessageService } from '../../_services/message.service';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { FormsModule } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from '../../_models/message';
import { RouterLink } from '@angular/router';
import { PaginationModule } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-messages',
  standalone: true,
  imports: [
    ButtonsModule,
    PaginationModule,
    FormsModule,
    TimeagoModule,
    RouterLink,
  ],
  templateUrl: './messages.component.html',
  styleUrl: './messages.component.css',
})
export class MessagesComponent implements OnInit {
  messageService = inject(MessageService);
  container = 'Unread';
  pageNumber = 1;
  pageSize = 5;
  get isOutbox(): boolean {
    return this.container === 'Outbox';
  }
  ngOnInit(): void {
    this.loadMessages();
  }
  loadMessages() {
    this.messageService.getMessages(
      this.pageNumber,
      this.pageSize,
      this.container
    );
  }
  deleteMessage(id: number) {
    this.messageService.deleteMessage(id).subscribe({
      next: (_) =>
        this.messageService.paginatedResult.update((prev) => {
          if (prev && prev.items) {
            const index = prev.items.findIndex((x) => x.id === id);
            if (index !== -1) prev.items.splice(index, 1);
            return prev;
          }
          return prev;
        }),
    });
  }
  getRouter(message: Message) {
    if (this.container === 'Outbox')
      return `/members/${message.recipientUsername}`;
    else return `/members/${message.senderUsername}`;
  }
  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadMessages();
    }
  }
}
