import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';

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

  private httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    withCredentials: true,
    observe: 'response' as 'response'
  };
  private res: number = 0

  constructor(private http: HttpClient) {
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
      r = (await firstValueFrom(this.http.post<any>(environment.apiUrl + '/Authentication/login', {email: email, password: password}, this.httpOptions))).ok
    } catch (e) {
      console.log(e)
    }
    return r
  }
  async register(email: string, firstName: string, lastName: string, roleId: string) {
    var r = false
    try {
      r = (await firstValueFrom(this.http.post<any>(environment.apiUrl + '/Registration/CreateUser', {email: email, firstName: firstName, lastName: lastName, roleId: roleId}, this.httpOptions))).ok
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
          this.httpOptions
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

  async logout() {

    var r = false
    try {
      r = (await firstValueFrom(this.http.post<any>(environment.apiUrl + '/Authentication/logout', {}, this.httpOptions))).ok
    } catch (e) {
      console.log(e)
    }

    // Update the currentUserSubject
    // this.currentUserSubject.next(null);

    return r
  }
}
