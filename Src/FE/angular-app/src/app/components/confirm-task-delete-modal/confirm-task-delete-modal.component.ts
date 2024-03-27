import { Component, Inject } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { TaskService } from '../../services/task.service';

type Data = {
  taskId: number
}

@Component({
  selector: 'app-confirm-task-delete-modal',
  standalone: true,
  imports: [MaterialModule],
  templateUrl: './confirm-task-delete-modal.component.html',
  styleUrl: './confirm-task-delete-modal.component.css'
})
export class ConfirmTaskDeleteModalComponent {
  constructor(
    private taskService: TaskService,
    public dialogRef: MatDialogRef<ConfirmTaskDeleteModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Data,
 ) {}

 async deleteTask(){
  await this.taskService.deleteTask(this.data.taskId);
  this.dialogRef.close();
 }
}
