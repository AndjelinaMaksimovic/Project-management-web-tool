import { Component, Input } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { ProjectItemComponent } from '../../components/project-item/project-item.component';
import { NgIf } from '@angular/common';
import { StatusItemComponent } from '../../components/status-item/status-item.component';
import { ProgressbarComponent } from '../../components/progressbar/progressbar.component';
import { ActivityItemComponent } from '../../components/activity-item/activity-item.component';
import { Project, ProjectService } from '../../services/project.service';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-project-details',
  standalone: true,
  imports: [ NavbarComponent, ProjectItemComponent, NgIf, StatusItemComponent, ProgressbarComponent, ActivityItemComponent, DatePipe ],
  templateUrl: './project-details.component.html',
  styleUrl: './project-details.component.scss'
})
export class ProjectDetailsComponent {
  project?: Project;

  projectId: number = 0;
  title?: string = "";
  description?: string = "";
  dueDate?: Date = new Date();
  daysLeft : number = 0;

  constructor(private projectService: ProjectService, private route: ActivatedRoute) { }

  async ngOnInit() {
    this.route.params.subscribe((params) => {
      this.projectId = parseInt(params['id']);
    });
    await this.projectService.fetchProjects();
    this.project = this.projectService.getProjectWithID(this.projectId);

    this.title = this.project?.title;
    this.description = this.project?.description;
    this.dueDate = this.project?.dueDate;

    let difference = this.dueDate!.getTime() - new Date().getTime();
    this.daysLeft = Math.floor(difference / (1000 * 60 * 60 * 24));
  }
}
