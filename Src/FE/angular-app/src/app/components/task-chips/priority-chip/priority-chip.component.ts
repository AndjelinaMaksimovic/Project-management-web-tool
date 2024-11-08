import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Task, TaskService } from '../../../services/task.service';
import { MaterialModule } from '../../../material/material.module';
import { PriorityService } from '../../../services/priority.service';

@Component({
  selector: 'app-priority-chip',
  standalone: true,
  imports: [MaterialModule],
  templateUrl: './priority-chip.component.html',
  styleUrl: './priority-chip.component.css',
})
export class PriorityChipComponent {
  @Input() task: Task | undefined;
  @Input() role: any = {};

  @Output() notifyUpdate: EventEmitter<void> = new EventEmitter<void>();

  constructor(private taskService: TaskService, private priorityService: PriorityService) {}

  _priorities: {id: number, name: string}[] = []
  get priorities(){
    if(this._priorities.length != this.priorityService.getPriorities().map(p => ({id: p.id, name: p.name})).length){
      this._priorities = this.priorityService.getPriorities().map(p => ({id: p.id, name: p.name}))
    }
    return this._priorities;
  }
  // priorities = this.priorityService.getPriorities().map(p => ({id: p.id, name: p.name}));

  async updatePriority(priorityId: string) {
    // console.log(priorityId);
    if (!this.task) return;
    await this.taskService.updateTask({
      id: this.task.id,
      priorityId: priorityId,
    });
    if(this.notifyUpdate) {
      this.notifyUpdate.emit();
    }
  }
}
