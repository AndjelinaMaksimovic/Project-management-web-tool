import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  /** TODO connect with backend; this is only for testing */
  private _tasks = [
    {
      title: 'Task name 1',
      category: 'Finance',
      priority: 'Low',
      status: 'Active',
      id: 1,
      date: new Date('2024'),
    },
    {
      title: 'Task 2',
      category: 'Finance',
      priority: 'Low',
      status: 'Active',
      id: 2,
      date: new Date('2024'),
    },
    {
      title: 'Task 3',
      category: 'Marketing',
      priority: 'High',
      status: 'Past Due',
      id: 3,
      date: new Date('2024'),
    },
    {
      title: 'Task 4',
      category: 'Finance',
      priority: 'High',
      status: 'Closed',
      id: 4,
      date: new Date('2024'),
    },
    {
      title: 'Task name 4',
      category: 'Finance',
      priority: 'Medium',
      status: 'Review',
      id: 5,
      date: new Date('2024'),
    },
    {
      title: 'Task name 5',
      category: 'Finance',
      priority: 'Medium',
      status: 'Review',
      id: 6,
      date: new Date('2024'),
    },
  ] as const;

  getTasks() {
    return this._tasks;
  }

  constructor() {}
}
