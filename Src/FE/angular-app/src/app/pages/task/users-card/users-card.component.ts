import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { Task, TaskService } from '../../../services/task.service';
import { MaterialModule } from '../../../material/material.module';
import { AvatarService } from '../../../services/avatar.service';
import { UserService } from '../../../services/user.service';
import { NgIf } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { UserStatsComponent } from '../../../components/user-stats/user-stats.component';

@Component({
  selector: 'app-users-card',
  standalone: true,
  imports: [ MaterialModule, NgIf ],
  templateUrl: './users-card.component.html',
  styleUrl: './users-card.component.css'
})
export class UsersCardComponent implements OnChanges {
  @Input() taskUsers!: any | undefined
  @Input() taskId: number | undefined
  @Input() projectId?: number
  @Input() role: any = {};
  users: any[] = []
  newUsers: any[] = []

  constructor(private dialog: MatDialog, private taskService: TaskService, private avatarService: AvatarService, private userService: UserService){}

  async ngOnChanges(changes: SimpleChanges) {
    if(changes['taskUsers']?.currentValue)
      this.users = await Promise.all(this.taskUsers.map(async (user: any) => {return await this.mapOther(user)}))
  }

  /**
   * Get and set profile and role for user
   * */
  async mapOther(user: any){
    user.profilePicture = this.avatarService.getProfileImagePath(user.id)
    user.role = (await this.userService.userRole(user.id)).roleName
    return user
  }
  
  async removeUser(user: any){
    if(!this.taskUsers || !this.taskId)
      return
    
    const tmp = [...this.users]
    tmp.splice(this.users.indexOf(user), 1)
    
    // if(await this.taskService.updateTask({id: this.task.id, userIds: tmp.map(usr => usr.id)}))
    if(await this.taskService.updateTask({id: this.taskId, userId: tmp[0].id}))
      this.users.splice(this.users.indexOf(user), 1)
  }

  
  openMember(id: number) {
    const dialogRef = this.dialog.open(UserStatsComponent, {
      panelClass: 'borderless-dialog',
      data: {
        id: id,
        title: "User details on project",
        projectId: this.projectId
      },
      maxHeight: '90vh'
    });
  }
}
