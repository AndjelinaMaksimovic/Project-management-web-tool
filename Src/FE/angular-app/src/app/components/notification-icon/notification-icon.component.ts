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
export class NotificationIconComponent implements OnInit, OnDestroy {
  activities: any[] = []
  menuOpened = false
  intervalId!: any
  

  constructor(private projectService: ProjectService, private socketService: SocketService){}

  async ngOnInit(){
    this.activities = await this.projectService.allUsersProjectActivities()
    this.activities = this.activities.sort((a:any, b:any) => a.time > b.time ? -1 : 1)

    // this.intervalId = setInterval(async () => {
    //   if(!this.menuOpened){
    //     console.log("fetch")
    //     this.activities = await this.projectService.allUsersProjectActivities()
    //     this.activities = this.activities.sort((a:any, b:any) => a.time > b.time ? -1 : 1)
    //   }
    // }, 10_000)

    this.socketService.ordersUpdated$.subscribe((notifications: any[])=>{
      this.activities.push(...notifications)
      this.activities = this.activities.sort((a:any, b:any) => a.time > b.time ? -1 : 1)
    })
  }

  ngOnDestroy(): void {
    clearInterval(this.intervalId)
  }
}
