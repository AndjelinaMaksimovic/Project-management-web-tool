import { Component, OnDestroy, OnInit } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { CommonModule } from '@angular/common';
import { MatMenuModule } from '@angular/material/menu';
import { ProjectService } from '../../services/project.service';
import { NotificationItemComponent } from '../notification-item/notification-item.component';
import { SocketService } from '../../services/socket.service';

@Component({
  selector: 'app-notification-icon',
  standalone: true,
  imports: [ MaterialModule, CommonModule, MatMenuModule, NotificationItemComponent ],
  templateUrl: './notification-icon.component.html',
  styleUrl: './notification-icon.component.css'
})
export class NotificationIconComponent implements OnInit {
  activities: any[] = []
  _activities: any[] = []
  menuOpened = false
  intervalId!: any
  seen = true
  

  constructor(private projectService: ProjectService, private socketService: SocketService){}

  async ngOnInit(){

    this.fetch()

    this.socketService.ordersUpdated$.subscribe((notification: any)=>{
      this.activities.push(notification)
      this._activities.push(notification)
      this.activities = this.activities.sort((a:any, b:any) => a.time > b.time ? -1 : 1)
      this._activities = this._activities.sort((a:any, b:any) => a.time > b.time ? -1 : 1)
      this.seen = false
    })
  }

  async fetch(){
    this._activities = await this.projectService.allUsersProjectActivities()
    this._activities = this._activities.sort((a:any, b:any) => a.time > b.time ? -1 : 1)
    this.activities = this._activities.filter(act => !act.seen)
    this.seen = this.activities.length == 0
  }

  onOpen(){
    if(this.activities.length == 0)
      return
    this.projectService.notificationsSeen()
    this.activities.forEach(notif => notif.seen = true)
    this.seen = true
  }
  onClose(){
    // no function for after closed so timeout is used
    setTimeout(()=>{this.activities = []}, 100)
  }

  loadMore(){
    const from = this.activities.length
    const to = Math.min(from + 4, this._activities.length)

    this.activities.push(...this._activities.slice(from, to))
  }
}
