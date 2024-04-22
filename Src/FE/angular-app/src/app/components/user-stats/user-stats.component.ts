import { Component, Input } from '@angular/core';
import { StatusItemComponent } from '../status-item/status-item.component';
import { ProjectItemComponent } from '../project-item/project-item.component';
import { TaskComponent } from '../../pages/task/task.component';
import { ProjectService } from '../../services/project.service';
import { TaskService } from '../../services/task.service';
import { NgIf } from '@angular/common';
import { TaskCardComponent } from '../task-card/task-card.component';

@Component({
  selector: 'app-user-stats',
  standalone: true,
  imports: [ StatusItemComponent, TaskComponent, ProjectItemComponent, NgIf, TaskCardComponent ],
  templateUrl: './user-stats.component.html',
  styleUrl: './user-stats.component.css'
})
export class UserStatsComponent {
  @Input() title: string = "";
  @Input() name: string = "";
  @Input() desc: string = "";
  @Input() projectId: number = -1;
  @Input() userId: number = -1;

  allTasksAccordionVisible: boolean = true;
  allProjectsAccordionVisible: boolean = true;

  get tasks() {
    return this.taskService.getTasks();
  }

  get projects(){
    return this.projectService.getProjects().filter(project => project.title.toLowerCase()).filter(project => !project.archived);
  }

  constructor(private projectService: ProjectService, private taskService: TaskService) {
    this.projectService.fetchUserProjects(this.userId);
    this.taskService.fetchUserTasks({ projectId: this.projectId, assignedTo: this.userId });
  }

  toggleTasks() {
    this.allTasksAccordionVisible = !this.allTasksAccordionVisible;
  }
  
  toggleProjects() {
    this.allProjectsAccordionVisible = !this.allProjectsAccordionVisible;
  }
}
