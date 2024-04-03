import { Component, OnInit, Input } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { NgIf } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon'
import { MatListModule } from '@angular/material/list'
import { animate, state, style, transition, trigger } from '@angular/animations';
import { TopnavComponent } from '../topnav/topnav.component';
import { NgStyle } from '@angular/common';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [MaterialModule, NgIf, RouterLink, RouterLinkActive, MatSidenavModule, MatIconModule, MatListModule, TopnavComponent, NgStyle ],
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
export class NavbarComponent{
  @Input() title?: string
  navMargin = 13;

  constructor(private router: Router){}
  
  isExpanded = true
  
  toggleNav() {
    this.isExpanded = !this.isExpanded;

    if(this.isExpanded) {
      this.navMargin = 13;
    }
    else {
      this.navMargin = 3.75;
    }
  }
}