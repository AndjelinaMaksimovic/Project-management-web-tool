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
    // Dummy credentials
    // const dummyCredentials = { email: 'user', password: 'pass' };

    // Check if the provided credentials match the dummy credentials
    // if (
    //   email === dummyCredentials.email &&
    //   password === dummyCredentials.password
    // ) {
    //   const user = { email: 'user', role: 'admin' };
    //   this.saveUserData(user);
    //   return true;
    // } else {
    //   // If login fails, return false
    //   return false;
    // }
    // const v = console.log(firstValueFrom(this.http.post<any>(environment.apiUrl + '/Authentication/login', {email: email, password: password}, this.httpOptions)))
    var r = false
    try {
      r = (await firstValueFrom(this.http.post<any>(environment.apiUrl + '/Authentication/login', {email: email, password: password}, this.httpOptions))).ok
    } catch (e) {
      console.log(e)
    }
    return r
  }
  async register(email: string, firstName: string, lastName: string) {
    // const user = { email: 'registered user', role: 'admin' };
    // this.saveUserData(user);
    // return true;
    // TODO connect with the backend

    // const registrationUrl = `${BASE_URL}/Registration/CreateUser/${token}/${email}/${password}`;
    // const requestOptions = {
    //   method: 'POST', // Specify the method
    //   headers: {
    //     'Content-Type': 'application/json', // Indicate the content type
    //   },
    // };
    // const res = await fetch(registrationUrl, requestOptions);
    // return res;
    var r = false
    try {
      r = (await firstValueFrom(this.http.post<any>(environment.apiUrl + '/Registration/CreateUser', {email: email, firstName: firstName, lastName: lastName, roleId: 0}, this.httpOptions))).ok
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
