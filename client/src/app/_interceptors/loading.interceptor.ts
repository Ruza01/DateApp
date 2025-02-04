import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../_services/busy.service';
import { delay, finalize } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService);

  busyService.busy(); //pocinje loading spiner

  return next(req).pipe(  //kada dobijemo response, prestaje loading spiner
    delay(1000),
    finalize(() => {
      busyService.idle()
    })
  )
};
