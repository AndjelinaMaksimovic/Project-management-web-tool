import { Component, Input } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { ProjectItemComponent } from '../../components/project-item/project-item.component';
import { NgIf } from '@angular/common';
import { StatusItemComponent } from '../../components/status-item/status-item.component';
import { ProgressbarComponent } from '../../components/progressbar/progressbar.component';
import { ActivityItemComponent } from '../../components/activity-item/activity-item.component';

@Component({
  selector: 'app-project-details',
  standalone: true,
  imports: [ NavbarComponent, ProjectItemComponent, NgIf, StatusItemComponent, ProgressbarComponent, ActivityItemComponent ],
  templateUrl: './project-details.component.html',
  styleUrl: './project-details.component.scss'
})
export class ProjectDetailsComponent {

}
