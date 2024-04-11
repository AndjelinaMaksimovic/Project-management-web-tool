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
  selector: 'app-create-category-modal',
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
  templateUrl: './create-category-modal.component.html',
  styleUrl: './create-category-modal.component.css',
})
export class CreateCategoryModalComponent {
  errorMessage: string | null = null;
  name: string | null = null;

  constructor(
    private taskService: TaskService,
    private categoryService: CategoryService,
    public dialogRef: MatDialogRef<CreateCategoryModalComponent>
  ) {}

  async createCategory() {
    if (!this.name) {
      this.errorMessage = 'Please provide all required fields';
      return;
    }
    await this.categoryService.createCategory(this.name);
    this.dialogRef.close();
  }
}
// import { Component } from '@angular/core';

// @Component({
//   selector: 'app-create-category-modal',
//   standalone: true,
//   imports: [],
//   templateUrl: './create-category-modal.component.html',
//   styleUrl: './create-category-modal.component.css'
// })
// export class CreateCategoryModalComponent {

// }
