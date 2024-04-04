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

@Component({
  selector: 'app-task-creation-modal',
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
  templateUrl: './task-creation-modal.component.html',
  styleUrl: './task-creation-modal.component.css',
})
export class TaskCreationModalComponent {
  errorMessage: string | null = null;
  title: string | null = null;
  description: string | null = null;
  date: string | null = null;
  startDate: string | null = "01/01/2024";
  priority: string | null = null;
  category: string | null = null;

  priorities = [
    { value: 'Low', viewValue: 'Low' },
    { value: 'Medium', viewValue: 'Medium' },
    { value: 'High', viewValue: 'High' },
  ];
  categories = [
    { value: 'Finance', viewValue: 'Finance' },
    { value: 'Marketing', viewValue: 'Marketing' },
    { value: 'Development', viewValue: 'Development' },
  ];

  constructor(
    private taskService: TaskService,
    public dialogRef: MatDialogRef<TaskCreationModalComponent>
  ) {}

  async createTask() {
    if (
      !this.title ||
      !this.date ||
      !this.priority ||
      !this.description ||
      !this.category
    ) {
      this.errorMessage = 'Please provide all required fields';
      return;
    }
    await this.taskService.createTask(
      {
        title: this.title,
        description: this.description,
        date: new Date(this.date),
        category: this.category,
        priority: this.priority as 'Low' | 'High' | 'Medium',
        status: 'Active',
      },
      1
    );
    this.dialogRef.close();
  }
}