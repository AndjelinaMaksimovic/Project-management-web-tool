import { Component, Inject } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

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
    @Inject(MAT_DIALOG_DATA) public data: Data
 ) {}

 async deleteTask(){
  // TODO implement task service; connect with backend
  console.log(this.data.taskId);
 }
}
