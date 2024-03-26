import { Component, Input } from '@angular/core';
import { MaterialModule } from '../../material/material.module';

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
