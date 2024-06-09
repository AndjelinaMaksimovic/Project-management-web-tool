import { Component, Input } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmTaskDeleteModalComponent } from '../confirm-task-delete-modal/confirm-task-delete-modal.component';
import { Task } from '../../services/task.service';
import { RouterModule } from '@angular/router';
import { NgIf } from '@angular/common';
import { AvatarStackComponent } from '../avatar-stack/avatar-stack.component';

@Component({
  selector: 'app-task-card',
  standalone: true,
  imports: [MaterialModule, RouterModule, NgIf, AvatarStackComponent],
  templateUrl: './task-card.component.html',
  styleUrl: './task-card.component.css',
})
export class TaskCardComponent {
  @Input() task!: Task;
  @Input() role: any = {};

  get asignees(){
    return this.task.assignedTo;
  }
  constructor(private dialog: MatDialog) {}

  deleteTask(){
    this.dialog.open(ConfirmTaskDeleteModalComponent, { autoFocus: false, data: this.task })
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
    const priorityColorMap: Partial<Record<string, string>> = {
      Finished: 'good',
      Active: 'mid',
      'Past Due': 'warn',
    } as const;
    return priorityColorMap[this.task.status] || "neutral";
  }
}
