import { Component, Input } from '@angular/core';
import {
  CdkDragDrop,
  CdkDrag,
  CdkDropList,
  CdkDropListGroup,
  moveItemInArray,
  transferArrayItem,
} from '@angular/cdk/drag-drop';
import { KanbanTaskCardComponent } from '../kanban-task-card/kanban-task-card.component';
import { Task } from '../../services/task.service';

@Component({
  selector: 'app-kanban-view',
  standalone: true,
  imports: [KanbanTaskCardComponent, CdkDropListGroup, CdkDropList, CdkDrag ],
  templateUrl: './kanban-view.component.html',
  styleUrl: './kanban-view.component.css',
})
export class KanbanViewComponent {
  _tasks: Task[] = [];
  @Input()
  public set tasks(
    val: Task[]
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

  drop(event: CdkDragDrop<Task[]>) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      // transferArrayItem(
      //   event.previousContainer.data,
      //   event.container.data,
      //   event.previousIndex,
      //   event.currentIndex,
      // );
    }
  }
}
