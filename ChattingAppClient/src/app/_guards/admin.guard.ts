import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);
  if (
    accountService.roles().includes('admin') ||
    accountService.roles().includes('moderator')
  )
    return true;
  else {
    toastr.error('accessing is not allowed');
    return false;
  }
};
