import { Component } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { LogoutButtonComponent } from '../logout-button/logout-button.component';
import { NgIf } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [MaterialModule, LogoutButtonComponent, NgIf, RouterLink, RouterLinkActive],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent {
  constructor(private router: Router, private auth: AuthService){}
  loggedIn(){
    return document.cookie.includes("sessionId")
  }
  async logout(){
    await this.auth.logout()
    this.router.navigate(['login'])
  }
}
