import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { AvatarService } from '../../services/avatar.service';

class Activity {
  constructor(
    public id: number,
    public projectId: number,
    public userId: number,
    public activityDescription: string,
    public userName: string = '',
    public userRole: string = '',
    public userIcon: string = '',
    public date: string = '',
  ){}
}

@Component({
  selector: 'app-activity-item',
  standalone: true,
  imports: [ CommonModule],
  templateUrl: './activity-item.component.html',
  styleUrl: './activity-item.component.scss'
})
export class ActivityItemComponent implements OnInit {
  @Input() activity: any

  constructor(private userService: UserService, private avatarService: AvatarService){}

  async ngOnInit() {
      const usr = await this.userService.getUser(this.activity.userId)
      this.activity.userName = usr.firstname + ' ' + usr.lastname
      this.activity.userRole = usr.roleName
      this.activity.userIcon = this.avatarService.getProfileImagePath(this.activity.userId)
  }
}
