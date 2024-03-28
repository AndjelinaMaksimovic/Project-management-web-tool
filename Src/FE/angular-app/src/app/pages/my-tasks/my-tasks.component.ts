import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../material/material.module';
import { ClearableInputComponent } from '../../components/clearable-input/clearable-input.component';
import { KanbanViewComponent } from '../../components/kanban-view/kanban-view.component';
import { TasksTableComponent } from '../../components/tasks-table/tasks-table.component';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { TaskService } from '../../services/task.service';
import { MatDialog } from '@angular/material/dialog';
import { TaskCreationModalComponent } from '../../components/task-creation-modal/task-creation-modal.component';

@Component({
  selector: 'app-my-tasks',
  standalone: true,
  imports: [
    FormsModule,
    CommonModule,
    MaterialModule,
    ClearableInputComponent,
    KanbanViewComponent,
    TasksTableComponent,
    NavbarComponent,
  ],
  templateUrl: './my-tasks.component.html',
  styleUrl: './my-tasks.component.css',
})
export class MyTasksComponent {
  constructor(private taskService: TaskService, private dialog: MatDialog) {}

  ngOnInit(){
    this.taskService.fetchTasks();
  }
  get tasks(){
    return this.taskService.getTasks();
  }
  /** this determines what task view we render */
  view: 'table' | 'kanban' | 'gantt' = 'kanban';

  createTask(){
    this.dialog.open(TaskCreationModalComponent)
  }
}
