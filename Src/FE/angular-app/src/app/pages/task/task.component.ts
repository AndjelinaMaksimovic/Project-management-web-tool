import { Component } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { ActivatedRoute } from '@angular/router';
import { Task, TaskService } from '../../services/task.service';
@Component({
  selector: 'app-task',
  standalone: true,
  imports: [
    NavbarComponent,
  ],
  templateUrl: './task.component.html',
  styleUrl: './task.component.css'
})
export class TaskComponent {
  taskId: number = 0;
  task: Task | undefined;
  constructor(
    private taskService: TaskService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.params.subscribe((params) => {
      this.taskId = parseInt(params['id']);
    });
    this.task = this.taskService.getTasks().find((t) => t.id === this.taskId);
  }
}



// import { Component } from '@angular/core';
// import { FormsModule } from '@angular/forms';
// import { CommonModule } from '@angular/common';
// import { MaterialModule } from '../../material/material.module';
// import { ClearableInputComponent } from '../../components/clearable-input/clearable-input.component';
// import { KanbanViewComponent } from '../../components/kanban-view/kanban-view.component';
// import { TasksTableComponent } from '../../components/tasks-table/tasks-table.component';
// import { NavbarComponent } from '../../components/navbar/navbar.component';
// import { TaskService } from '../../services/task.service';
// import { MatDialog } from '@angular/material/dialog';
// import { TaskCreationModalComponent } from '../../components/task-creation-modal/task-creation-modal.component';
// import { ActivatedRoute } from '@angular/router';
// import { GanttComponent } from '../../components/gantt/gantt.component';
// import { StatusService } from '../../services/status.service';
// import { CreateStatusModalComponent } from '../../components/create-status-modal/create-status-modal.component';

// @Component({
//   selector: 'app-my-tasks',
//   standalone: true,
//   imports: [
//     FormsModule,
//     CommonModule,
//     MaterialModule,
//     ClearableInputComponent,
//     KanbanViewComponent,
//     TasksTableComponent,
//     NavbarComponent,
//     GanttComponent,
//   ],
//   templateUrl: './my-tasks.component.html',
//   styleUrl: './my-tasks.component.css',
// })
// export class MyTasksComponent {
//   projectId: number = 0;
//   constructor(
//     private taskService: TaskService,
//     private statusService: StatusService,
//     private dialog: MatDialog,
//     private route: ActivatedRoute
//   ) {}

//   ngOnInit() {
//     this.route.params.subscribe((params) => {
//       this.projectId = parseInt(params['id']);
//     });
//     this.taskService.fetchTasks({ projectId: this.projectId });
//   }
//   get tasks() {
//     return this.taskService.getTasks();
//   }
//   /** this determines what task view we render */
//   view: 'table' | 'kanban' | 'gantt' = 'table';

//   createTask() {
//     this.dialog.open(TaskCreationModalComponent);
//   }

//   createStatus() {
//     this.dialog.open(CreateStatusModalComponent);
//   }
// }
