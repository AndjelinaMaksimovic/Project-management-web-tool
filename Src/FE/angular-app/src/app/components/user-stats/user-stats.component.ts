import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { Component, Inject, Input } from '@angular/core';
import { StatusItemComponent } from '../status-item/status-item.component';
import { ProjectItemComponent } from '../project-item/project-item.component';
import { TaskComponent } from '../../pages/task/task.component';
import { ProjectService } from '../../services/project.service';
import { TaskService } from '../../services/task.service';
import { NgIf } from '@angular/common';
import { TaskCardComponent } from '../task-card/task-card.component';
import { UserService } from '../../services/user.service';
import { AvatarService } from '../../services/avatar.service';

@Component({
  selector: 'app-user-stats',
  standalone: true,
  imports: [ StatusItemComponent, TaskComponent, ProjectItemComponent, NgIf, TaskCardComponent, MatDialogModule ],
  templateUrl: './user-stats.component.html',
  styleUrl: './user-stats.component.css'
})
export class UserStatsComponent {
  @Input() title: string = "";
  @Input() projectId: number = -1;
  @Input() userId: number = -1;

  name: string = "";
  desc: string = "";
  image: string = "";

  allTasksAccordionVisible: boolean = true;
  allProjectsAccordionVisible: boolean = true;

  tasksVisible: boolean = false;

  get tasks() {
    return this.taskService.getTasks();
  }

  get projects(){
    return this.projectService.getProjects().filter(project => project.title.toLowerCase()).filter(project => !project.archived);
  }

  constructor(private userService: UserService, private projectService: ProjectService, private avatarService: AvatarService, private taskService: TaskService, @Inject(MAT_DIALOG_DATA) public data: any) {
    this.userId = data.id;
    this.title = data.title;
    this.projectId = data.projectId ? data.projectId : -1;
  }

  async ngOnInit() {
    let user = await this.userService.getUser(this.userId);
    console.log(user);
    this.name = user.firstname + " " + user.lastname;
    this.desc = user.roleName;
    this.image = user.profilePicture;

    this.projectService.fetchUserProjects(this.userId);
    if(this.projectId != -1) {
      console.log("daaa");
      this.tasksVisible = true;
      await this.taskService.fetchUserTasks({ projectId: this.projectId, assignedTo: this.userId });
      console.log(this.tasks);
    }
  }

  toggleTasks() {
    this.allTasksAccordionVisible = !this.allTasksAccordionVisible;
  }
  
  toggleProjects() {
    this.allProjectsAccordionVisible = !this.allProjectsAccordionVisible;
  }

  getProfileImagePath(){
    return this.avatarService.getProfileImagePath(this.userId);
  }
}
