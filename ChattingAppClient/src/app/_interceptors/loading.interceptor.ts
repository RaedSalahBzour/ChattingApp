import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../_services/busy.service';
import { delay, finalize } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const bussyService = inject(BusyService);
  bussyService.busy();
  return next(req).pipe(
    delay(500),
    finalize(() => {
      bussyService.idle();
    })
  );
};
