import { Component, Input } from '@angular/core';
import { Task, TaskService } from '../../../services/task.service';
import { MaterialModule } from '../../../material/material.module';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import moment from 'moment';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-assignee-chip',
  standalone: true,
  imports: [
    MaterialModule,
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
  ],
  templateUrl: './assignee-chip.component.html',
  styleUrl: './assignee-chip.component.css'
})
export class AssigneeChipComponent {
  @Input() task: Task | undefined;
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


// import { Component } from '@angular/core';

// @Component({
//   selector: 'app-assignee-chip',
//   standalone: true,
//   imports: [],
//   templateUrl: './assignee-chip.component.html',
//   styleUrl: './assignee-chip.component.css'
// })
// export class AssigneeChipComponent {

// }
