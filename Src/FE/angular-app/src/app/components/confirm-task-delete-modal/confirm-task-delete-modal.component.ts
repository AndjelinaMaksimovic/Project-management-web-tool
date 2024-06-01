import { Component, Inject } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Task, TaskService } from '../../services/task.service';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-confirm-task-delete-modal',
  standalone: true,
  imports: [MaterialModule, NgIf],
  templateUrl: './confirm-task-delete-modal.component.html',
  styleUrl: './confirm-task-delete-modal.component.css'
})
export class ConfirmTaskDeleteModalComponent {
  constructor(
    private taskService: TaskService,
    public dialogRef: MatDialogRef<ConfirmTaskDeleteModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Task,
 ) {}

 async deleteTask(){
  await this.taskService.deleteTask(this.data.id);
  this.dialogRef.close();
 }
}
