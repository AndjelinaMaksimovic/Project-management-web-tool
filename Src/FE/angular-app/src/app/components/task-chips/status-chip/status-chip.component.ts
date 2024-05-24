import { Component, Input } from '@angular/core';
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

  constructor(private statusService: StatusService, private taskService: TaskService){}

  get statuses(){
    return this.statusService.getStatuses();
  }

  updateStatus(statusId: string){
    console.log(statusId);
    if(!this.task) return;
    this.taskService.updateTask({
      id: this.task.id,
      statusId: statusId
    })
  }
}
