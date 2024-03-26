import { Component, Input } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmTaskDeleteModalComponent } from '../confirm-task-delete-modal/confirm-task-delete-modal.component';

@Component({
  selector: 'app-kanban-task-card',
  standalone: true,
  imports: [MaterialModule],
  templateUrl: './kanban-task-card.component.html',
  styleUrl: './kanban-task-card.component.css',
})
export class KanbanTaskCardComponent {
  @Input() task!: Readonly<{
    title: string;
    priority: 'High' | 'Medium' | 'Low';
    category: string;
    status: 'Finished' | 'Active' | 'Past Due';
    date: Date;
    id: number;
  }>;

  constructor(private dialog: MatDialog) {}

  deleteTask() {
    this.dialog.open(ConfirmTaskDeleteModalComponent, {
      autoFocus: false,
      data: { taskId: this.task.id },
    });
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
