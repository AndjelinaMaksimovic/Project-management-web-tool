import { Component, OnDestroy, OnInit } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { CommonModule } from '@angular/common';
import { MatMenuModule } from '@angular/material/menu';
import { ProjectService } from '../../services/project.service';
import { NotificationItemComponent } from '../notification-item/notification-item.component';

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
  seen = true
  

  constructor(private projectService: ProjectService){}

  async ngOnInit(){
    this.fetch()
    this.intervalId = setInterval(async () => {
      if(!this.menuOpened){
        this.fetch()
      }
    }, 10_000)
  }

  async fetch(){
    this.activities = await this.projectService.allUsersProjectActivities()
    this.seen = !this.activities.some(act => !act.seen)
  }

  ngOnDestroy(): void {
    clearInterval(this.intervalId)
  }

  onOpen(){
    this.seen = true
    this.projectService.notificationsSeen()
  }
}
