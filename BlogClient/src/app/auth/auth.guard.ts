// auth.guard.ts

import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthserviceService } from './authservice.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: AuthserviceService, private router: Router) {}
  
  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    const { token } = this.authService.getToken();

  
    if (this.authService.isLoggedIn || this.authService.isAdministrator) {
  debugger
  console.log(this.authService.isLoggedIn)
      if (state.url.startsWith('/admin-page') ) {
        this.router.navigate(['/home']);
        alert('You do not have permission to access Admins page, please login!');

        return false;
      }
  
      if (state.url.startsWith('/client-page') ) {
        this.router.navigate(['/home']);
        alert('You do not have permission to access this page, please login!');
      }
  
      return false;
    }
  
    alert('You do not have permission to access this page, please login!');
    this.router.navigate(['']);
    return false;
  }
    
  }
