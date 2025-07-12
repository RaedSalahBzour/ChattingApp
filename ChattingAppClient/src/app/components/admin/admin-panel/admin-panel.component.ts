import { Component } from '@angular/core';
import { UserManagementComponent } from '../user-management/user-management.component';
import { PhotoManagementComponent } from '../photo-management/photo-management.component';
import { HasRoleDirective } from '../../../_directives/has-role.directive';
import { TabsModule } from 'ngx-bootstrap/tabs';

@Component({
  selector: 'app-admin-panel',
  standalone: true,
  imports: [
    UserManagementComponent,
    PhotoManagementComponent,
    HasRoleDirective,
    TabsModule,
  ],
  templateUrl: './admin-panel.component.html',
  styleUrl: './admin-panel.component.css',
})
export class AdminPanelComponent {}
