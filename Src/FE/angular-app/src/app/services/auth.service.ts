import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot } from '@angular/router';

type User = {
  email: string;
  role: string;
};

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private currentUserSubject: BehaviorSubject<User | null>;
  public currentUser: Observable<User | null>;
  private res: number = 0

  constructor(private http: HttpClient, private router: Router) {
    this.currentUserSubject = new BehaviorSubject<User | null>(
      JSON.parse(localStorage.getItem('currentUser') || 'null')
    );
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  private saveUserData(user: User) {
    // save user data to localStorage
    localStorage.setItem('currentUser', JSON.stringify(user));
    // Update the currentUserSubject
    this.currentUserSubject.next(user);
  }

  async login(email: string, password: string) {
    var r = false
    try {
      r = (await firstValueFrom(this.http.post<any>(environment.apiUrl + '/Authentication/login', {email: email, password: password}, environment.httpOptions))).ok
      // clear current user id cache on logout
      this._myId = undefined;
    } catch (e) {
      console.log(e)
    }
    return r
  }
  async register(email: string, firstName: string, lastName: string, roleId: string) {
    var r = false
    try {
      r = (await firstValueFrom(this.http.post<any>(environment.apiUrl + '/Registration/CreateUser', {email: email, firstName: firstName, lastName: lastName, roleId: roleId}, environment.httpOptions))).ok
    } catch (e) {
      console.log(e)
    }
    return r
  }

  async activate(
    token: string,
    email: string,
    password: string
  ): Promise<boolean> {
    try {
      const res = await firstValueFrom(
        this.http.post<any>(
          environment.apiUrl +
            `/Registration/Activate/${token}/${email}/${password}`,
          {},
          environment.httpOptions
        )
      );
      if (!res.ok) return false;
      const success = await this.login(email, password);
      if(!success) return false;
      return true;
    } catch (e) {
      console.log(e);
    }
    return false;
  }

  // we cache current logged in users id value
  _myId: number | undefined;
  public async getMyId() {
    if(this._myId !== undefined) return this._myId;
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl + `/User/getMyData`,
          environment.httpOptions
        )
      );
      this._myId = res.body.id;
    } catch (e) {
      console.log(e);
    }
    return this._myId;
  }
  async logout() {
    var r = false
    try {
      r = (await firstValueFrom(this.http.post<any>(environment.apiUrl + '/Authentication/logout', {}, environment.httpOptions))).ok
      this.router.navigate(['login']);
      // clear current user id cache on logout
      this._myId = undefined;
    } catch (e) {
      console.log(e)
    }

    // Update the currentUserSubject
    // this.currentUserSubject.next(null);

    return r
  }
  loggedIn(route: ActivatedRouteSnapshot, state: RouterStateSnapshot){
    if(!document.cookie.includes("sessionId")){
      this.router.navigate(["/login"])
      return false
    }
    return true
  }
  notLoggedIn(route: ActivatedRouteSnapshot, state: RouterStateSnapshot){
    if(!document.cookie.includes("sessionId")){
      return true
    }
    this.router.navigate(["/"])
    return false
  }
}

export const LoggedIn: CanActivateFn = (next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean => {
  return inject(AuthService).loggedIn(next, state);
}
export const NotLoggedIn: CanActivateFn = (next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean => {
  return inject(AuthService).notLoggedIn(next, state);
}
