import { Component } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { AuthService } from '../../services/auth.service';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ClearableInputComponent } from '../../components/clearable-input/clearable-input.component';
import { EmailFieldComponent } from '../../components/email-field/email-field.component';
import { SelectComponent } from '../../components/select/select.component';
import { RolesService } from '../../services/roles.service';
import { Task, TaskService } from '../../services/task.service';
import { MatDialogRef } from '@angular/material/dialog';
import { CategoryService } from '../../services/category.service';
import { MAT_DATE_LOCALE, provideNativeDateAdapter } from '@angular/material/core';
@Component({
  selector: 'app-task-creation-modal',
  standalone: true,
  imports: [
    MaterialModule,
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    ClearableInputComponent,
    EmailFieldComponent,
    SelectComponent,
  ],
  templateUrl: './task-creation-modal.component.html',
  styleUrl: './task-creation-modal.component.css',
  providers: [ provideNativeDateAdapter(), { provide: MAT_DATE_LOCALE, useValue: 'en-GB' } ]
})
export class TaskCreationModalComponent {
  errorMessage: string | null = null;
  title: string | null = null;
  description: string | null = null;
  // date: string | null = null;
  // startDate: string | null = null;
  dueDate = new FormControl(new Date())
  startDate = new FormControl(new Date())
  priority: string | null = null;
  category: string | null = null;
  dependencies: string[] = [];

  priorities = [
    { value: 'Low', viewValue: 'Low' },
    { value: 'Medium', viewValue: 'Medium' },
    { value: 'High', viewValue: 'High' },
  ];
  categories: { value: string; viewValue: string }[] = [];
  tasks = this.taskService.getTasks().map(t => ({value: t.id, viewValue: t.title}));
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
      !this.dueDate ||
      !this.priority ||
      !this.description ||
      !this.category ||
      !this.dueDate.value ||
      !this.startDate.value
    ) {
      this.errorMessage = 'Please provide all required fields';
      return;
    }
    await this.taskService.createTask(
      {
        title: this.title,
        description: this.description,
        date: this.dueDate.value,
        category: this.category,
        priority: this.priority as 'Low' | 'High' | 'Medium',
        status: 'Active',
        assignedTo: [],
        dependencies: this.dependencies,
      },
      1
    );
    this.dialogRef.close();
  }
}