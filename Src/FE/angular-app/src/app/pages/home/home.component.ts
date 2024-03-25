import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon'
import { MatBadgeModule } from '@angular/material/badge'
import { DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { NgStyle } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { InviteModalComponent } from '../../components/invite-modal/invite-modal.component';
import { TopnavComponent } from '../../components/topnav/topnav.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ NavbarComponent, MatSidenavModule, MatIconModule, MatBadgeModule, DatePipe, MatButtonModule, NgStyle, RouterModule, TopnavComponent ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent{
  username: string = '';
  constructor(private authService: AuthService, private dialog: MatDialog) {
    this.username = authService.currentUserValue?.email || '';
  }

  
  user = {
    name: "Petar",
    surname: "Petrovic"
  }
  date = new Date()

  projects = [
    {
      title: 'Project 1',
      desc: 'Lorem ipsum dolor sit',
      progress: 48
    },
    {
      title: 'Project 1',
      desc: 'Lorem ipsum dolor sit',
      progress: 48
    }
  ]

  addAccount(){
    this.dialog.open(InviteModalComponent, { autoFocus: false })
  }
}
