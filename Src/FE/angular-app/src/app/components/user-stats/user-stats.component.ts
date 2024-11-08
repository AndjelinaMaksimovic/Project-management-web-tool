import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
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
import { ContributionGraphComponent } from '../contribution-graph/contribution-graph.component';

@Component({
  selector: 'app-user-stats',
  standalone: true,
  imports: [ StatusItemComponent, TaskComponent, ProjectItemComponent, NgIf, TaskCardComponent, MatDialogModule, ContributionGraphComponent ],
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
  
  allTasks : number = 0;
  completedTasks : number = 0;
  overdueTasks : number = 0;

  allTasksAccordionVisible: boolean = true;
  allProjectsAccordionVisible: boolean = true;
  activityCalendarAccordionVisible: boolean = true;

  tasksVisible: boolean = false;
  
  activities: any[] = [];
  activityData?: number[];

  get tasks() {
    return this.taskService.getTasks();
  }

  get projects(){
    return this.projectService.getProjects().filter(project => project.title.toLowerCase()).filter(project => !project.archived);
  }

  constructor(private userService: UserService, private projectService: ProjectService, private avatarService: AvatarService, private taskService: TaskService, @Inject(MAT_DIALOG_DATA) public data: any, public dialogRef: MatDialogRef<UserStatsComponent>) {
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
      this.tasksVisible = true;
      await this.taskService.fetchUserTasks({ projectId: this.projectId, assignedTo: this.userId });
    }
    // else {
    //   await this.taskService.fetchUserTasks({ assignedTo: this.userId });
    // }

    this.allTasks = this.taskService.getTasks().length;
    this.completedTasks = this.taskService.getTasks().filter((task) => task.status == "Done").length;
    this.overdueTasks = this.taskService.getTasks().filter((task) => new Date(task.dueDate) < new Date()).length;

    this.activities = await this.projectService.allUserActivitiesById(this.userId);
    this.activities = this.activities.sort((a: any, b: any) => a.time > b.time ? -1 : 1)
    this.activityData = this.activities.map((a) => {
      return new Date(a.time).getTime();
    });
  }

  toggleTasks() {
    this.allTasksAccordionVisible = !this.allTasksAccordionVisible;
  }

  toggleActivity() {
    this.activityCalendarAccordionVisible = !this.activityCalendarAccordionVisible;
  }

  toggleProjects() {
    this.allProjectsAccordionVisible = !this.allProjectsAccordionVisible;
  }

  getProfileImagePath(){
    return this.avatarService.getProfileImagePath(this.userId);
  }

  closeDialog() {
    this.dialogRef.close();
  }
}
