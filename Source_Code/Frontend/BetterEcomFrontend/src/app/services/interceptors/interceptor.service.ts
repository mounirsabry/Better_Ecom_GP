import { Injectable } from '@angular/core';
import {HttpErrorResponse, HttpInterceptor} from '@angular/common/http'
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { Router } from '@angular/router';
@Injectable({
  providedIn: 'root'
})
export class InterceptorService implements HttpInterceptor{

  constructor(private router:Router) { }

  intercept(req, next){

    let tokenizedRequest = req.clone({

      setHeaders:{
        Authorization: 'Bearer ' + localStorage.getItem('token')
      }
    })

    return next.handle(tokenizedRequest).pipe(
      catchError(
        (error:HttpErrorResponse) => {
          //unauthorized

          if(error.status == 401){
            // which login to go to login/student or the other.
            this.router.navigate(['startPage',{errorMessage:"you are not authorized to do this action, plz login to the apropriate role"}])
          }

          //['./',{errorMessage:"you entered either wrong id or password"}],{relativeTo: this.activatedRoute}
          // for some reason i need to put this line or else there will be compilation errro.
          return throwError(error)
        }
      )
    )
  }
}
