import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { AvatarService } from '../../services/avatar.service';
import { UserStatsComponent } from '../user-stats/user-stats.component';
import { MatDialog } from '@angular/material/dialog';
import { MarkdownModule, provideMarkdown } from 'ngx-markdown';

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
  imports: [ CommonModule, MarkdownModule ],
  providers: [provideMarkdown()],
  templateUrl: './activity-item.component.html',
  styleUrl: './activity-item.component.scss'
})
export class ActivityItemComponent implements OnInit {
  @Input() activity: any

  constructor(private dialog: MatDialog, private userService: UserService, private avatarService: AvatarService){}

  async ngOnInit() {
      const usr = await this.userService.getUser(this.activity.userId)
      this.activity.userName = usr.firstname + ' ' + usr.lastname
      this.activity.userRole = usr.roleName
      this.activity.userIcon = this.avatarService.getProfileImagePath(this.activity.userId)
  }
  
  openMember(id: number) {
    const dialogRef = this.dialog.open(UserStatsComponent, {
      panelClass: 'borderless-dialog',
      data: {
        id: id,
        title: "User details on project",
        projectId: this.activity.projectId
      },
      maxHeight: '90vh'
    });
  }
}
