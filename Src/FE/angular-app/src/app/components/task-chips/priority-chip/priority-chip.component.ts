import { Component, Input } from '@angular/core';
import { Task, TaskService } from '../../../services/task.service';
import { MaterialModule } from '../../../material/material.module';

@Component({
  selector: 'app-priority-chip',
  standalone: true,
  imports: [MaterialModule],
  templateUrl: './priority-chip.component.html',
  styleUrl: './priority-chip.component.css',
})
export class PriorityChipComponent {
  @Input() task: Task | undefined;

  constructor(private taskService: TaskService) {}

  priorities = [
    { name: 'Low', id: 1 },
    { name: 'Medium', id: 2 },
    { name: 'High', id: 3 },
  ];

  updatePriority(priorityId: string) {
    // console.log(priorityId);
    if (!this.task) return;
    this.taskService.updateTask({
      id: this.task.id,
      priorityId: priorityId,
    });
  }
}
