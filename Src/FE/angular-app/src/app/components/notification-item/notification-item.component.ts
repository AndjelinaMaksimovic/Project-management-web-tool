import { Component, Input } from '@angular/core';
import { UserService } from '../../services/user.service';
import { AvatarService } from '../../services/avatar.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-notification-item',
  standalone: true,
  imports: [ CommonModule ],
  templateUrl: './notification-item.component.html',
  styleUrl: './notification-item.component.css'
})
export class NotificationItemComponent {
  @Input() activity: any

  constructor(private userService: UserService, private avatarService: AvatarService){}

  async ngOnInit() {
      const usr = await this.userService.getUser(this.activity.userId)
      this.activity.userName = usr.firstname + ' ' + usr.lastname
      this.activity.userRole = usr.roleName
      this.activity.userIcon = this.avatarService.getProfileImagePath(this.activity.userId)
  }
}
