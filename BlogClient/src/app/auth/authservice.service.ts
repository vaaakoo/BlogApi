import { HttpClient} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Useregisteration } from './useregisteration';

import { environment } from '../../environments/environment';


@Injectable({
  providedIn: 'root',
})
export class AuthserviceService {
  constructor(private http: HttpClient) {}

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);

  private apiUrl: string = environment.apiUrl;

  private authToken: string = '';



  get isAuthenticated(): Observable<boolean> {
    return this.isAuthenticatedSubject.asObservable();
  }

  setAuthenticationToken(token: string): void {
    this.authToken = token;
    localStorage.setItem('authToken', token);
    this.isAuthenticatedSubject.next(true);
  }

  clearAuthenticationToken(): void {
    this.authToken = '';
    localStorage.removeItem('authToken');
    localStorage.removeItem('userInfo');
    this.isAuthenticatedSubject.next(false);
  }

  logout(): void {
    this.clearAuthenticationToken();
  }

  getToken(): { token: string, userInfo: any } {
    const token = localStorage.getItem('authToken') || '';    
    const userInfo = this.getUserInfo();
    return { token, userInfo };
  }

  // 
  setUserInfo(user: any): void {
    localStorage.setItem('userInfo', JSON.stringify(user));
  }
  
  

  getUserInfo(): any {
    const userInfoString = localStorage.getItem('userInfo');
    return userInfoString ? JSON.parse(userInfoString) : null;
  }
  

  // 
  login(user: Useregisteration): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/Authentication/login`, user)
      .pipe(
        tap(response => {
          this.setAuthenticationToken(response.token);
          const user = response.user;
          this.setUserInfo(user);
        })
      );
  }
  

  get isLoggedIn(): boolean {
    const { userInfo } = this.getToken();
    return userInfo && userInfo.role === 'client';
  }

  get isAdministrator(): boolean {
    const { userInfo } = this.getToken();
    return userInfo && userInfo.role === 'admin';
  }

  RegistrationUser(user: Useregisteration): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/User/register`, user);
  }

 
  getUserById(id: number): Observable<Useregisteration> {
    return this.http.get<Useregisteration>(`${this.apiUrl}/getUser/${id}`);
  }

  setAuthenticationStatus(isAuthenticated: boolean): void {
    this.isAuthenticatedSubject.next(isAuthenticated);
  }

}
