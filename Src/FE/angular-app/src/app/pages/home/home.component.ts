import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon'
import { MatBadgeModule } from '@angular/material/badge'
import { DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { NgStyle } from '@angular/common';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ NavbarComponent, MatSidenavModule, MatIconModule, MatBadgeModule, DatePipe, MatButtonModule, NgStyle ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent {
  username: string = '';
  constructor(private authService: AuthService) {
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
}
