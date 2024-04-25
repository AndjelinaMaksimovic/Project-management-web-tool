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
import { TaskService } from '../../services/task.service';
import { MatDialog } from '@angular/material/dialog';

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
  progress : number = 0;

  allTasks : number = 0;
  completedTasks : number = 0;
  overdueTasks : number = 0;

  constructor(private projectService: ProjectService, private route: ActivatedRoute, private taskService: TaskService, public dialog: MatDialog) {
    this.dialog.closeAll();
  }

  async ngOnInit() {
    this.route.params.subscribe((params) => {
      this.projectId = parseInt(params['id']);
    });
    await this.projectService.fetchProjects();
    this.project = this.projectService.getProjectWithID(this.projectId);

    await this.taskService.fetchTasks({ projectId: this.projectId });

    this.title = this.project?.title;
    this.description = this.project?.description;
    this.dueDate = this.project?.dueDate;

    let difference = this.dueDate!.getTime() - new Date().getTime();
    this.daysLeft = Math.floor(difference / (1000 * 60 * 60 * 24));

    const _progress = this.projectService.getProgress(this.projectId);
    if(_progress != null && _progress != undefined) {
      this.progress = _progress;
    }
    this.allTasks = this.taskService.getTasks().length;
    this.completedTasks = this.taskService.getTasks().filter((task) => task.status == "Done").length;
    this.overdueTasks = this.taskService.getTasks().filter((task) => new Date(task.dueDate) < new Date()).length;

    console.log(this.taskService.getTasks());
  }
}
