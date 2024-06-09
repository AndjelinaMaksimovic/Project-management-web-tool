import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
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
export class DateChipComponent implements OnChanges {
  @Input() task: Task | undefined;
  @Input() role: any = {};
  dueDate = new FormControl(moment());

  constructor(private taskService: TaskService) {
    this.dueDate = new FormControl(moment(this.task?.dueDate));
  }
  
  ngOnChanges(changes: SimpleChanges): void {
    if(changes['task']){
      this.dueDate.setValue(changes['task'].currentValue.dueDate)
    }
  }

  updateDate() {
    if (!this.task) return;

    const offset = this.dueDate.value!.utcOffset()
    this.dueDate.value!.add(moment.duration(offset, 'minutes'))

    this.taskService.updateTask({
      id: this.task.id,
      dueDate: this.dueDate?.value?.toDate(),
    });
  }
}
