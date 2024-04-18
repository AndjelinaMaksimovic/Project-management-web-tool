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
import { ActivatedRoute, Router } from '@angular/router';
import { GanttComponent } from '../../components/gantt/gantt.component';
import { StatusService } from '../../services/status.service';
import { CreateStatusModalComponent } from '../../components/create-status-modal/create-status-modal.component';
import { CreateCategoryModalComponent } from '../../components/create-category-modal/create-category-modal.component';
import { MilestoneService } from '../../services/milestone.service';
import { FiltersComponent } from '../../components/filters/filters.component';
import { Filter } from '../../components/filters/filters.component';
import { PriorityService } from '../../services/priority.service';

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
  filters: Map<string, Filter> = new Map<string, Filter>();

  isFilterOpen: boolean = false;

  projectId: number = 0;
  constructor(
    private taskService: TaskService,
    private statusService: StatusService,
    private milestoneService: MilestoneService,
    private priorityService: PriorityService,
    private dialog: MatDialog,
    private route: ActivatedRoute,
    private router: Router,
  ) {}

  async ngOnInit() {
    await this.route.params.subscribe((params) => {
      this.projectId = parseInt(params['id']);
    });
    this.taskService.fetchTasksFromLocalStorage(this.projectId, "task_filters");
    this.milestoneService.fetchMilestones({ projectId: this.projectId });
    
    this.statusService.setContext({ projectId: this.projectId });
    await this.statusService.fetchStatuses();
    await this.priorityService.fetchPriorities();

    this.filters = new Map<string, Filter>([
      ["DueDateAfter", new Filter({ name: 'Start date', icon: 'fa-regular fa-calendar', type: 'date' })],
      ["DueDateBefore", new Filter({ name: 'Due date', icon: 'fa-solid fa-flag-checkered', type: 'date' })],
      // ["AssignedTo", new Filter({ name: 'Assigned to', icon: 'fa-solid fa-user', type: 'select', items: [ new Item({ value: "1", name: "Test" })]})],
      ["StatusId", new Filter({ name: 'Status', icon: 'fa-solid fa-circle-exclamation', type: 'select', items: this.statusService.getStatuses().map(status => ({ value: status.id, name: status.name }))})],
      ["PriorityId", new Filter({ name: 'Priority', icon: 'fa-solid fa-arrow-up', type: 'select', items: this.priorityService.getPriorities().map(priority => ({ value: priority.id, name: priority.name }))})]
    ]);
  }
  get tasks() {
    return this.taskService.getTasks();
  }
  get milestones() {
    return this.milestoneService.getMilestones();
  }
  /** this determines what task view we render */
  view: 'table' | 'kanban' | 'gantt' = 'table';

  fetchTasksFromLocalStorage() {
    this.taskService.fetchTasksFromLocalStorage(this.projectId, "task_filters");
  }

  createTask() {
    this.router.navigateByUrl(`/project/${this.projectId}/new-task`);
  }

  createStatus() {
    this.dialog.open(CreateStatusModalComponent);
  }
  createCategory() {
    this.dialog.open(CreateCategoryModalComponent);
  }

  openFilters() {
    this.isFilterOpen = !this.isFilterOpen;
  }
}
