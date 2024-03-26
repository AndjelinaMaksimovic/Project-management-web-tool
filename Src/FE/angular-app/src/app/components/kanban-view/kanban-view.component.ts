import { Component, Input } from '@angular/core';
import { KanbanTaskCardComponent } from '../kanban-task-card/kanban-task-card.component';

@Component({
  selector: 'app-kanban-view',
  standalone: true,
  imports: [KanbanTaskCardComponent],
  templateUrl: './kanban-view.component.html',
  styleUrl: './kanban-view.component.css',
})
export class KanbanViewComponent {
  _tasks: Readonly<
    {
      title: string;
      priority: 'High' | 'Medium' | 'Low';
      category: string;
      status: string;
      date: Date;
      id: number;
    }[]
  > = [];
  @Input()
  public set tasks(
    val: Readonly<
      {
        title: string;
        priority: 'High' | 'Medium' | 'Low';
        category: string;
        status: string;
        date: Date;
        id: number;
      }[]
    >
  ) {
    this._tasks = val;
    this.statuses = Array.from(
      val.reduce((acc, task) => {
        acc.add(task.status);
        return acc;
      }, new Set<string>())
    );
  }
  get tasks() {
    return this._tasks;
  }
  statuses: string[] = [];

  getTasksOfStatus(status: (typeof this.tasks)[number]['status']) {
    return this.tasks.filter((t) => t.status === status);
  }
}
