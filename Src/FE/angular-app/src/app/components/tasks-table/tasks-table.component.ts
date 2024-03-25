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
    { name: 'Task1', category: 'finance', priority: 'Low', status: 'Finished' },
    { name: 'Task2', category: 'finance', priority: 'High', status: 'Active' },
    { name: 'Task 3', category: 'finance', priority: 'High', status: 'Finished' },
  ] as const;
}
