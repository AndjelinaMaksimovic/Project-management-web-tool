import { Component, Input } from '@angular/core';
import { Task, TaskService } from '../../../services/task.service';
import { MaterialModule } from '../../../material/material.module';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ClearableInputComponent } from '../../../components/clearable-input/clearable-input.component';

@Component({
  selector: 'app-updatable-title',
  standalone: true,
  imports: [
    MaterialModule,
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    ClearableInputComponent,
  ],
  templateUrl: './updatable-title.component.html',
  styleUrl: './updatable-title.component.css'
})
export class UpdatableTitleComponent {
  @Input() task: Task | undefined = undefined;
  title: string = "";

  constructor(private taskService: TaskService) {}

  updateTitle() {
    if (!this.task) return;
    this.taskService.updateTask({
      id: this.task.id,
      title: this.title,
    });
  }
}

// import { Component } from '@angular/core';

// @Component({
//   selector: 'app-updatable-title',
//   standalone: true,
//   imports: [],
//   templateUrl: './updatable-title.component.html',
//   styleUrl: './updatable-title.component.css'
// })
// export class UpdatableTitleComponent {

// }
