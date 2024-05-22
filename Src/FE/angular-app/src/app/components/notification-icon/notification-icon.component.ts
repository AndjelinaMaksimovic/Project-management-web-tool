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
  

  constructor(private projectService: ProjectService){}

  async ngOnInit(){
    this.activities = await this.projectService.allUsersProjectActivities()
    this.intervalId = setInterval(async () => {
      if(!this.menuOpened){
        console.log("fetch")
        this.activities = await this.projectService.allUsersProjectActivities()
      }
    }, 10_000)
  }

  ngOnDestroy(): void {
    clearInterval(this.intervalId)
  }
}
