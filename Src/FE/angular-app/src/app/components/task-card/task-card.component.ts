import { Component, Input } from '@angular/core';
import { MaterialModule } from '../../material/material.module';

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
