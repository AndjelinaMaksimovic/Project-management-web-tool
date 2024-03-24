import { Component } from '@angular/core';
import { MaterialModule } from '../../material/material.module';

@Component({
  selector: 'app-confirm-task-delete-modal',
  standalone: true,
  imports: [MaterialModule],
  templateUrl: './confirm-task-delete-modal.component.html',
  styleUrl: './confirm-task-delete-modal.component.css'
})
export class ConfirmTaskDeleteModalComponent {

}
