import { Component } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ClearableInputComponent } from '../../components/clearable-input/clearable-input.component';
import { EmailFieldComponent } from '../../components/email-field/email-field.component';
import { SelectComponent } from '../../components/select/select.component';
import { RolesService } from '../../services/roles.service';
import { Task, TaskService } from '../../services/task.service';
import { MatDialogRef } from '@angular/material/dialog';
import { StatusService } from '../../services/status.service';

@Component({
  selector: 'app-status-rename-modal',
  standalone: true,
  imports: [
    MaterialModule,
    FormsModule,
    CommonModule,
    ClearableInputComponent,
    EmailFieldComponent,
    MaterialModule,
    SelectComponent,
  ],
  templateUrl: './status-rename-modal.component.html',
  styleUrl: './status-rename-modal.component.css'
})
export class StatusRenameModalComponent {
  errorMessage: string | null = null;
  name: string | null = null;

  constructor(
    private taskService: TaskService,
    private statusService: StatusService,
    public dialogRef: MatDialogRef<StatusRenameModalComponent>
  ) {}

  async createTask() {
    if (
      !this.name
    ) {
      this.errorMessage = 'Please provide all required fields';
      return;
    }
    await this.statusService.createStatus(this.name);
    this.dialogRef.close();
  }
}