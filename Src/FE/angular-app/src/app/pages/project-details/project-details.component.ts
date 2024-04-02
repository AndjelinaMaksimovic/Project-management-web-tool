import { Component, Input } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { ProjectItemComponent } from '../../components/project-item/project-item.component';
import { NgIf } from '@angular/common';
import { StatusItemComponent } from '../../components/status-item/status-item.component';
import { ProgressbarComponent } from '../../components/progressbar/progressbar.component';

@Component({
  selector: 'app-project-details',
  standalone: true,
  imports: [ NavbarComponent, ProjectItemComponent, NgIf, StatusItemComponent, ProgressbarComponent ],
  templateUrl: './project-details.component.html',
  styleUrl: './project-details.component.scss'
})
export class ProjectDetailsComponent {

}
