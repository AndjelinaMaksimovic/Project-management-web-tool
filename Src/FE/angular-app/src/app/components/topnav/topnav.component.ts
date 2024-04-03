import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-topnav',
  standalone: true,
  imports: [ MatButtonModule, MatIconModule, MatInputModule, MatMenuModule ],
  templateUrl: './topnav.component.html',
  styleUrl: './topnav.component.css'
})

export class TopnavComponent {
  constructor(private authService: AuthService, private router: Router){ }

  async logOut(){
    await this.authService.logout()
    this.router.navigate(['login'])
  }
}
