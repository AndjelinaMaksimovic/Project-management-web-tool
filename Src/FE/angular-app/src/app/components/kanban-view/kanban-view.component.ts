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
import { Task, TaskService } from '../../services/task.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-kanban-view',
  standalone: true,
  imports: [KanbanTaskCardComponent, CdkDropListGroup, CdkDropList, CdkDrag, CommonModule ],
  templateUrl: './kanban-view.component.html',
  styleUrl: './kanban-view.component.css',
})
export class KanbanViewComponent {
  constructor(private taskService: TaskService){}
  mobile: boolean = false;
  ngOnInit() {
    if (window.screen.width < 600) { // 768px portrait
      this.mobile = true;
    } else {
      this.mobile = false;
    }
  }
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
    ).sort();
  }
  get tasks() {
    return this._tasks;
  }
  statuses: string[] = [];

  getTasksOfStatus(status: (typeof this.tasks)[number]['status']) {
    return this.tasks.filter((t) => t.status === status);
  }

  async drop(event: CdkDragDrop<Task[]>) {
    if (event.previousContainer === event.container) {
      // if we are in the same column, do nothing
    } else {
      console.log(event.container.id);
      console.log("this.getTasksOfStatus(event.previousContainer.id)[event.previousIndex]", this.getTasksOfStatus(event.previousContainer.id)[event.previousIndex])
      const task = this.getTasksOfStatus(event.previousContainer.id)[event.previousIndex];
      await this.taskService.updateTask({id: task.id, status: event.container.id});
    }
  }
}
