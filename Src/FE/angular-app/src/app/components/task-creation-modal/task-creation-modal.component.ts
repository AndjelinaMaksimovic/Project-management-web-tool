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
import { CategoryService } from '../../services/category.service';

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
  startDate: string | null = null;
  priority: string | null = null;
  category: string | null = null;

  priorities = [
    { value: 'Low', viewValue: 'Low' },
    { value: 'Medium', viewValue: 'Medium' },
    { value: 'High', viewValue: 'High' },
  ];
  categories: { value: string; viewValue: string }[] = [];
  // get categories(){
  //   return this.categoryService.getCategories().map(cat => {
  //     return {
  //       value: cat.id.toString(),
  //       viewValue: cat.name
  //     }
  //   })
  // }

  constructor(
    private taskService: TaskService,
    private categoryService: CategoryService,
    public dialogRef: MatDialogRef<TaskCreationModalComponent>
  ) {
    this.categories = this.categoryService.getCategories().map((cat) => {
      return {
        value: cat.id.toString(),
        viewValue: cat.name,
      };
    });
  }

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
        assignedTo: [],
      },
      1
    );
    this.dialogRef.close();
  }
}