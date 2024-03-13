import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

type User = {
  email: string;
  role: string;
};
/** auth server url */
const BASE_URL = 'https://localhost:7167';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private currentUserSubject: BehaviorSubject<User | null>;
  public currentUser: Observable<User | null>;

  constructor() {
    this.currentUserSubject = new BehaviorSubject<User | null>(
      JSON.parse(localStorage.getItem('currentUser') || 'null')
    );
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  login(email: string, password: string) {
    // Dummy credentials
    const dummyCredentials = { email: 'user', password: 'pass' };

    // Check if the provided credentials match the dummy credentials
    if (
      email === dummyCredentials.email &&
      password === dummyCredentials.password
    ) {
      // If successful, simulate storing user data in localStorage
      const user = { email: 'user', role: 'admin' };
      localStorage.setItem('currentUser', JSON.stringify(user));
      // Update the currentUserSubject
      this.currentUserSubject.next(user);
      return true;
    } else {
      // If login fails, return false
      return false;
    }
  }
  async register(token: string, email: string, password: string) {
    const registrationUrl = `${BASE_URL}/Registration/CreateUser/${token}/${email}/${password}`;
    const requestOptions = {
      method: 'POST', // Specify the method
      headers: {
        'Content-Type': 'application/json', // Indicate the content type
      },
    };
    const res = await fetch(registrationUrl, requestOptions);
    return res;
  }

  logout() {
    // Remove user data from localStorage
    localStorage.removeItem('currentUser');
    // Update the currentUserSubject
    this.currentUserSubject.next(null);
  }
}
