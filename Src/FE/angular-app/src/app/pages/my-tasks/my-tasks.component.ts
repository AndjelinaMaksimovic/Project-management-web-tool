import { Component } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { ClearableInputComponent } from '../../components/clearable-input/clearable-input.component';
import { KanbanViewComponent } from '../../components/kanban-view/kanban-view.component';
import { TasksTableComponent } from '../../components/tasks-table/tasks-table.component';

@Component({
  selector: 'app-my-tasks',
  standalone: true,
  imports: [MaterialModule, ClearableInputComponent, KanbanViewComponent, TasksTableComponent],
  templateUrl: './my-tasks.component.html',
  styleUrl: './my-tasks.component.css',
})
export class MyTasksComponent {
  tasks = [
    { title: 'Task name 1', category: 'Finance', priority: 'Low', status: 'Active', id: 1, date: new Date("2024") },
    { title: 'Task 2', category: 'Finance', priority: 'Low', status: 'Active', id: 1, date: new Date("2024") },
    { title: 'Task 3', category: 'Marketing', priority: 'High', status: 'Past Due', id: 1, date: new Date("2024") },
    { title: 'Task 4', category: 'Finance', priority: 'High', status: 'Closed', id: 1, date: new Date("2024") },
  ] as const;
}
