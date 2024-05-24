import { Component, Input } from '@angular/core';
import { Task, TaskService } from '../../../services/task.service';
import { MaterialModule } from '../../../material/material.module';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import moment from 'moment';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-date-chip',
  standalone: true,
  imports: [
    MaterialModule,
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
  ],
  templateUrl: './date-chip.component.html',
  styleUrl: './date-chip.component.css',
})
export class DateChipComponent {
  @Input() task: Task | undefined;
  @Input() role: any = {};
  dueDate = new FormControl(moment());

  constructor(private taskService: TaskService) {
    this.dueDate = new FormControl(moment(this.task?.dueDate));
  }

  updateDate() {
    if (!this.task) return;
    this.taskService.updateTask({
      id: this.task.id,
      dueDate: this.dueDate?.value?.toDate(),
    });
  }
}
