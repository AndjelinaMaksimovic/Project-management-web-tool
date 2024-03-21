import { Component, OnInit } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { NgIf } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon'
import { MatListModule } from '@angular/material/list'
import { animate, state, style, transition, trigger } from '@angular/animations';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [MaterialModule, NgIf, RouterLink, RouterLinkActive, MatSidenavModule, MatIconModule, MatListModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss',
  animations: [
    trigger('openClose', [
      state('opened', style({
        // width: '100px'
      })),
      state('closed', style({
        width: '60px'
      })),
      transition('opened <=> closed', [animate('0.2s'), ]),
    ])
  ]
})
export class NavbarComponent implements OnInit{

  constructor(private router: Router, private auth: AuthService){}
  
  ngOnInit(): void {
    if(!document.cookie.includes("sessionId")){
      this.router.navigate(['login'])
    }
  }
  
  isExpanded = false
  
  loggedIn(){
    return document.cookie.includes("sessionId")
  }
  async logout(){
    await this.auth.logout()
    this.router.navigate(['login'])
  }
}