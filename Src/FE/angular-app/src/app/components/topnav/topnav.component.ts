import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { AuthService } from '../../services/auth.service';
import { Router, RouterLink } from '@angular/router';
import { environment } from '../../../environments/environment';
import { AvatarService } from '../../services/avatar.service';
import { NotificationIconComponent } from '../notification-icon/notification-icon.component';
@Component({
  selector: 'app-topnav',
  standalone: true,
  imports: [ MatButtonModule, MatIconModule, MatInputModule, MatMenuModule, RouterLink, NotificationIconComponent ],
  templateUrl: './topnav.component.html',
  styleUrl: './topnav.component.css'
})

export class TopnavComponent {
  constructor(private authService: AuthService, private router: Router, public avatarService: AvatarService){ }
  loggedInUserId: number | undefined; 
  async ngOnInit(){
    this.loggedInUserId = await this.authService.getMyId();
  }
  getProfileImagePath(){
    return this.avatarService.getProfileImagePath(this.loggedInUserId)
  }
  async logOut(){
    await this.authService.logout()
  }
}
