import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { UserService } from './user.service';

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

  constructor(private http: HttpClient, private router: Router, private userService: UserService) {
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
        this.http.post<any>(environment.apiUrl + `/Invites/AcceptInvite`, 
          {
            token: token,
            email: email,
            password: password,
          },
          {...environment.httpOptions, responseType: "text" as "json"}
        )
      );
      if (!res.ok) return false;
      // const success = await this.login(email, password);
      // if(!success) return false;
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
  loggedIn(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree{
    console.log("loggedInCheck")
    if(!document.cookie.includes("sessionId")){
      return this.router.createUrlTree(["/login"])
    }
    return true
  }
  notLoggedIn(route: ActivatedRouteSnapshot, state: RouterStateSnapshot){
    console.log("notloggedInCheck")
    if(!document.cookie.includes("sessionId")){
      return true
    }
    return this.router.createUrlTree(["/"])
  }
  async canSeeCompanyMembers(route: ActivatedRouteSnapshot, state: RouterStateSnapshot){
    const role = await this.userService.currentUserRole()
    if(role.canAddUserToProject)
      return true
    return this.router.createUrlTree(["/"])
  }
  async notSuperUser(route: ActivatedRouteSnapshot, state: RouterStateSnapshot){
    const role = await this.userService.currentUserRole()
    if(role.canAddNewUser)
      return this.router.createUrlTree(["/company-members"])
    return true
  }
}

export const LoggedIn: CanActivateFn = (next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree => {
  return inject(AuthService).loggedIn(next, state);
}
export const NotLoggedIn: CanActivateFn = (next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree => {
  return inject(AuthService).notLoggedIn(next, state);
}
export const NotSuperUser: CanActivateFn = (next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean | UrlTree> => {
  return inject(AuthService).notSuperUser(next, state);
}
export const CanSeeCompanyMembers: CanActivateFn = (next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean | UrlTree> => {
  return inject(AuthService).canSeeCompanyMembers(next, state);
}
