import { Component, HostListener, Input } from '@angular/core';
import {
  CdkDragDrop,
  CdkDrag,
  CdkDropList,
  CdkDropListGroup,
} from '@angular/cdk/drag-drop';
import { KanbanTaskCardComponent } from '../kanban-task-card/kanban-task-card.component';
import { Task, TaskService } from '../../services/task.service';
import { CommonModule } from '@angular/common';
import { StatusService } from '../../services/status.service';
import { MaterialModule } from '../../material/material.module';

@Component({
  selector: 'app-kanban-view',
  standalone: true,
  imports: [KanbanTaskCardComponent, CdkDropListGroup, CdkDropList, CdkDrag, CommonModule, MaterialModule ],
  templateUrl: './kanban-view.component.html',
  styleUrl: './kanban-view.component.css',
})
export class KanbanViewComponent {
  constructor(private taskService: TaskService, private statusService: StatusService){}
  mobile: boolean = false;
  ngOnInit() {}
  @HostListener('window:resize', ['$event'])
  onResize(){
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
  }
  get tasks() {
    return this._tasks;
  }
  get statuses(){
    return this.statusService.getStatuses().map(s => s.name);
  }

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

  async deleteStatus(status: string){
    this.statusService.deleteStatus(status);
  }

  async reorderStatus(event: CdkDragDrop<string[]>){
    const movedStatus = this.statuses[event.previousIndex];
    const newOrder = [...this.statuses];
    newOrder.splice(event.previousIndex, 1);
    newOrder.splice(event.currentIndex, 0, movedStatus);
    await this.statusService.reorderStatuses(newOrder);
  }
}
