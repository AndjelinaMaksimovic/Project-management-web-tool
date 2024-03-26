import { Component, Input } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmTaskDeleteModalComponent } from '../confirm-task-delete-modal/confirm-task-delete-modal.component';

@Component({
  selector: 'app-task-card',
  standalone: true,
  imports: [MaterialModule],
  templateUrl: './task-card.component.html',
  styleUrl: './task-card.component.css',
})
export class TaskCardComponent {
  @Input() task!: {
    title: string;
    priority: 'High' | 'Medium' | 'Low';
    category: string;
    status: 'Finished' | 'Active' | 'Past Due';
    date: string;
    id: number;
  };

  constructor(private dialog: MatDialog) {}

  deleteTask(taskId: number){
    this.dialog.open(ConfirmTaskDeleteModalComponent, { autoFocus: false, data: {taskId: taskId} })
  }

  get priorityColor() {
    const priorityColorMap = {
      High: 'warn',
      Medium: 'mid',
      Low: 'neutral',
    } as const;
    return priorityColorMap[this.task.priority];
  }
  get statusColor() {
    const priorityColorMap = {
      Finished: 'good',
      Active: 'mid',
      'Past Due': 'warn',
    } as const;
    return priorityColorMap[this.task.status];
  }
}
