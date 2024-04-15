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
import { ActivatedRoute } from '@angular/router';
import { GanttComponent } from '../../components/gantt/gantt.component';
import { StatusService } from '../../services/status.service';
import { CreateStatusModalComponent } from '../../components/create-status-modal/create-status-modal.component';
import { FiltersComponent } from '../../components/filters/filters.component';
import { Filter } from '../../components/filters/filters.component';

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
    GanttComponent,
    FiltersComponent
  ],
  templateUrl: './my-tasks.component.html',
  styleUrl: './my-tasks.component.css',
})
export class MyTasksComponent {
  filters: Map<string, Filter> = new Map<string, Filter>([
    ["DueDateAfter", new Filter({ name: 'Start date', icon: 'fa-regular fa-calendar', type: 'date' })],
    ["DueDateBefore", new Filter({ name: 'Due date', icon: 'fa-solid fa-flag-checkered', type: 'date' })],
    // ["AssignedTo", new Filter({ name: 'Assigned to', icon: 'fa-solid fa-user', type: 'select', items: [ new Item({ value: "1", name: "Test" })]})],
  ]);

  isFilterOpen: boolean = false;

  projectId: number = 0;
  constructor(
    private taskService: TaskService,
    private statusService: StatusService,
    private dialog: MatDialog,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.params.subscribe((params) => {
      this.projectId = parseInt(params['id']);
    });
    this.taskService.fetchTasksFromLocalStorage(this.projectId, "task_filters");
  }
  get tasks() {
    return this.taskService.getTasks();
  }
  /** this determines what task view we render */
  view: 'table' | 'kanban' | 'gantt' = 'table';

  fetchTasksFromLocalStorage() {
    this.taskService.fetchTasksFromLocalStorage(this.projectId, "task_filters");
  }

  createTask() {
    this.dialog.open(TaskCreationModalComponent);
  }

  createStatus() {
    this.dialog.open(CreateStatusModalComponent);
  }

  openFilters() {
    this.isFilterOpen = !this.isFilterOpen;
  }
}
