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
import { InviteModalComponent } from '../invite-modal/invite-modal.component';
import { MatDialog } from '@angular/material/dialog';
import { UserService } from '../../services/user.service';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-topnav',
  standalone: true,
  imports: [ CommonModule, MatButtonModule, MatIconModule, MatInputModule, MatMenuModule, RouterLink, NotificationIconComponent ],
  templateUrl: './topnav.component.html',
  styleUrl: './topnav.component.css'
})

export class TopnavComponent {
  constructor(private userService: UserService, private dialogue: MatDialog, private authService: AuthService, private router: Router, public avatarService: AvatarService){ }
  loggedInUserId: number | undefined;
  role: any = {}
  async ngOnInit(){
    this.loggedInUserId = await this.authService.getMyId();
    this.role = await this.userService.currentUserRole()
  }
  getProfileImagePath(){
    return this.avatarService.getProfileImagePath(this.loggedInUserId)
  }
  async logOut(){
    await this.authService.logout()
  }
  invitePopup(){
    this.dialogue.open(InviteModalComponent, { autoFocus: false })
  }
}
