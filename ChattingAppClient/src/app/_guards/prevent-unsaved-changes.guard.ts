import { CanDeactivateFn } from '@angular/router';
import { EditProfileComponent } from '../components/members/edit-profile/edit-profile.component';
import { inject } from '@angular/core';
import { ConfirmService } from '../_services/confirm.service';

export const preventUnsavedChangesGuard: CanDeactivateFn<
  EditProfileComponent
> = (component) => {
  const confirmService = inject(ConfirmService);
  if (component.editForm?.dirty) {
    return confirmService.confirm() ?? false;
  }
  return true;
};
