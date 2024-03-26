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
    { title: 'Task1', category: 'finance', priority: 'Low', status: 'Finished', date: "teste", id: 1 },
    { title: 'Task2', category: 'finance', priority: 'High', status: 'Active', date: "teste", id: 2 },
    { title: 'Task 3', category: 'finance', priority: 'High', status: 'Finished', date: "teste", id: 3 },
  ] as const;
}
