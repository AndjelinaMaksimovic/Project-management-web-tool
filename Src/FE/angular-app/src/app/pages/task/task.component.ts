import { Component } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { ActivatedRoute } from '@angular/router';
import { Task, TaskService } from '../../services/task.service';
import { MarkdownModule, provideMarkdown } from 'ngx-markdown';
import { MaterialModule } from '../../material/material.module';
import { CommentsComponent } from '../../components/comments/comments/comments.component';
import { EditableMarkdownComponent } from '../../components/editable-markdown/editable-markdown.component';
import { CategoryChipComponent } from '../../components/task-chips/category-chip/category-chip.component';
import { StatusChipComponent } from '../../components/task-chips/status-chip/status-chip.component';
import { DateChipComponent } from '../../components/task-chips/date-chip/date-chip.component';
import { PriorityChipComponent } from '../../components/task-chips/priority-chip/priority-chip.component';
import { UpdatableTitleComponent } from './updatable-title/updatable-title.component';
import { AssigneeChipComponent } from '../../components/task-chips/assignee-chip/assignee-chip.component';
import { ProgressChipComponent } from '../../components/task-chips/progress-chip/progress-chip.component';
import { UsersCardComponent } from './users-card/users-card.component';
import { AddUserChipComponent } from '../../components/task-chips/add-user-chip/add-user-chip.component';
import { DependantTasksCardComponent } from './dependant-tasks-card/dependant-tasks-card.component';
import { UserService } from '../../services/user.service';
import { NgIf } from '@angular/common';
import { AddDependantTasksChipComponent } from '../../components/task-chips/add-dependant-tasks-chip/add-dependant-tasks-chip.component';
import { MatDialog } from '@angular/material/dialog';
@Component({
  selector: 'app-task',
  standalone: true,
  imports: [
    NavbarComponent,
    MarkdownModule,
    MaterialModule,
    CommentsComponent,
    EditableMarkdownComponent,
    CategoryChipComponent,
    StatusChipComponent,
    PriorityChipComponent,
    DateChipComponent,
    UpdatableTitleComponent,
    AssigneeChipComponent,
    ProgressChipComponent,
    UsersCardComponent,
    AddUserChipComponent,
    DependantTasksCardComponent,
    NgIf,
    AddDependantTasksChipComponent,
  ],
  providers: [provideMarkdown()],
  templateUrl: './task.component.html',
  styleUrl: './task.component.css',
})
export class TaskComponent {
  taskId: number = 0;
  projectId: number = 0;

  tasks?: Task[]
  // users: any[] = []
  // dependantTasks: any = []
  role: any = {}

  constructor(
    private taskService: TaskService,
    private route: ActivatedRoute,
    private userService: UserService,
    private dialog: MatDialog
  ) {
    this.dialog.closeAll();
  }

  get task() {
    const tasks = this.taskService.getTasks()
    const task = tasks.find((t) => t.id === this.taskId);
    return task
  }
  get dependantTasks(){
    return this.task?.dependentTasks.map((dep: any) => {
      const t = this.tasks?.find(_task => _task.id == dep.taskId)
      return {dependant: t, type: dep.typeOfDependencyId}
    })
  }
  get users(){
    return this.task?.assignedTo
  }
  set dependantTasks(p: any){};
  set users(p: any){}

  async ngOnInit() {
    this.route.params.subscribe((params) => {
      this.taskId = parseInt(params['taskId']);
      this.projectId = parseInt(params['id']);
      
    });
    await this.taskService.fetchTasks({ projectId: this.projectId });

    this.role = await this.userService.currentUserRole()
    this.tasks = this.taskService.getTasks()
    // this.users = this.task?.assignedTo
    // this.dependantTasks = task?.dependentTasks.map((dep) => {
    //   const t = this.tasks?.find(_task => _task.id == dep.taskId)
    //   return {dependant: t, type: dep.typeOfDependencyId}
    // })
  }

  updateDescription(newDescription: string) {
    this.taskService.updateTask({
      id: this.taskId,
      description: newDescription,
    });
  }
}
