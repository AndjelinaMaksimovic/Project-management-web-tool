import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { AuthService } from '../../services/auth.service';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-topnav',
  standalone: true,
  imports: [ MatButtonModule, MatIconModule, MatInputModule, MatMenuModule, RouterLink ],
  templateUrl: './topnav.component.html',
  styleUrl: './topnav.component.css'
})

export class TopnavComponent {
  constructor(private authService: AuthService, private router: Router){ }

  async logOut(){
    await this.authService.logout()
  }
}
