import { Injectable, Injector } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
//import Swal from 'sweetalert2';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {   
 
    constructor(private oidcSecurityService: OidcSecurityService) { }
 
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        let requestToForward = req;       
        
        if (this.oidcSecurityService !== undefined) {
            let token = this.oidcSecurityService.getToken();
            if (token !== '') {
                let tokenValue = 'Bearer ' + token;
                requestToForward = req.clone({ setHeaders: { Authorization: tokenValue } });
            }
        } else {
            console.debug('OidcSecurityService undefined: NO auth header!');
        }
 
        return next.handle(requestToForward)
        .pipe(
            catchError((error: HttpErrorResponse) => {
                let data = [];
                if (!navigator.onLine) {
                    data.push('Sem conexão com a internet!');
                } else {
                    if (error.status == 422) {
                        const errors = error.error.errors;
                        Object.getOwnPropertyNames(errors).forEach(element => {
                            data = data.concat(errors[element]);
                        });
                    } else if (error.status == 403) {
                        data.push('Acesso negado!');
                    }
                    else if (error.status == 401) {
                        this.oidcSecurityService.authorize();
                    }
                }

                if (data.length > 0) {
                    // Swal.fire({
                    //     title: 'Error!',
                    //     text: data.toString(),
                    //     icon: 'error',
                    //     confirmButtonText: 'OK'
                    // });
                }

                return throwError(error);
            }));
        
    }
}