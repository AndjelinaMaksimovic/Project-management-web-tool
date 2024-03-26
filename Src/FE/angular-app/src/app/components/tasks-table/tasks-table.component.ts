import { Component } from '@angular/core';
import { TaskCardComponent } from '../task-card/task-card.component';

@Component({
  selector: 'app-tasks-table',
  standalone: true,
  imports: [TaskCardComponent],
  templateUrl: './tasks-table.component.html',
  styleUrl: './tasks-table.component.css'
})
export class TasksTableComponent {
  tasks = [
    { title: 'Task1', category: 'finance', priority: 'Low', status: 'Finished', date: new Date("2023"), id: 1 },
    { title: 'Task2', category: 'finance', priority: 'High', status: 'Active', date: new Date("2023"), id: 2 },
    { title: 'Task 3', category: 'finance', priority: 'High', status: 'Finished', date: new Date("2023"), id: 3 },
  ] as const;
}
