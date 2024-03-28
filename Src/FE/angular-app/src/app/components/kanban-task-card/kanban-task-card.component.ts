import { Component, Input } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmTaskDeleteModalComponent } from '../confirm-task-delete-modal/confirm-task-delete-modal.component';
import { Task } from '../../services/task.service';

@Component({
  selector: 'app-kanban-task-card',
  standalone: true,
  imports: [MaterialModule],
  templateUrl: './kanban-task-card.component.html',
  styleUrl: './kanban-task-card.component.css',
})
export class KanbanTaskCardComponent {
  @Input() task!: Task;

  constructor(private dialog: MatDialog) {}

  deleteTask() {
    this.dialog.open(ConfirmTaskDeleteModalComponent, {
      autoFocus: false,
      data: this.task,
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
}
