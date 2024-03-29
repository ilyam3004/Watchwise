import {Injectable} from '@angular/core';
import { HttpInterceptor, HttpHandler, HttpRequest, HttpEvent }
  from '@angular/common/http';
import {Observable} from "rxjs";
import {environment} from "../../../environments/environment.development";
import {AccountService} from "../../core/services/account.service";

@Injectable()
export class ApiInterceptor implements HttpInterceptor {

  constructor(private accountService: AccountService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const apiReq = request
      .clone({ url: `${environment.apiBaseUrl}/${request.url}`});

    const user = this.accountService.userValue;
    const isLoggedIn = user && user.token;

    if (isLoggedIn) {
      const authRequest = apiReq.clone({
        setHeaders: {Authorization: `Bearer ${user.token}`}
      });

      return next.handle(authRequest);
    }

    return next.handle(apiReq);
  }
}
