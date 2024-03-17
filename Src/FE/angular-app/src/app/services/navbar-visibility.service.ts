import { Injectable } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class NavbarVisibilityService {
  constructor(private router: Router) {}

  shouldShowNavbar(): boolean {
    const NO_NAVBAR_URLS = ['/activate', '/login'];
    return !NO_NAVBAR_URLS.includes(this.router.url);
  }
}
