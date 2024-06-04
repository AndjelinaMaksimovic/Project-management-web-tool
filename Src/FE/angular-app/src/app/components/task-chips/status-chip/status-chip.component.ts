import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Task, TaskService } from '../../../services/task.service';
import { MaterialModule } from '../../../material/material.module';
import { StatusService } from '../../../services/status.service';

@Component({
  selector: 'app-status-chip',
  standalone: true,
  imports: [MaterialModule],
  templateUrl: './status-chip.component.html',
  styleUrl: './status-chip.component.css',
})
export class StatusChipComponent {
  @Input() task: Task | undefined;
  @Input() role: any = {}

  @Output() notifyUpdate: EventEmitter<void> = new EventEmitter<void>();

  constructor(private statusService: StatusService, private taskService: TaskService){}

  get statuses(){
    return this.statusService.getStatuses();
  }

  async updateStatus(statusId: string){
    console.log(statusId);
    if(!this.task) return;
    await this.taskService.updateTask({
      id: this.task.id,
      statusId: statusId
    })
    if(this.notifyUpdate) {
      this.notifyUpdate.emit();
    }
  }
}
