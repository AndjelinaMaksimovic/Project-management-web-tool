import { Component } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
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
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { CategoryService } from '../../services/category.service';
import {
  MAT_DATE_LOCALE,
  provideNativeDateAdapter,
} from '@angular/material/core';
import { UserService } from '../../services/user.service';
import { CreateCategoryModalComponent } from '../../components/create-category-modal/create-category-modal.component';
import { StatusService } from '../../services/status.service';
import { CreateStatusModalComponent } from '../../components/create-status-modal/create-status-modal.component';

@Component({
  selector: 'app-new-task',
  standalone: true,
  imports: [
    NavbarComponent,
    MaterialModule,
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    ClearableInputComponent,
    EmailFieldComponent,
    SelectComponent,
  ],
  providers: [
    provideNativeDateAdapter(),
    { provide: MAT_DATE_LOCALE, useValue: 'en-GB' },
  ],
  templateUrl: './new-task.component.html',
  styleUrl: './new-task.component.css',
})
export class NewTaskComponent {
  projectId: number | undefined;

  errorMessage: string | null = null;
  title: string | null = null;
  description: string | null = null;
  // date: string | null = null;
  // startDate: string | null = null;
  dueDate = new FormControl(new Date());
  startDate = new FormControl(new Date());
  priority: string | null = null;
  status: string = this.statusService.getStatuses()[0].id.toString();
  category: string | null = null;
  dependencies: string[] = [];
  assignee: string | undefined;

  tasks: { value: string; viewValue: string }[] = [];
  users: { value: string; viewValue: string }[] = [];

  priorities = [
    { value: '1', viewValue: 'Low' },
    { value: '2', viewValue: 'Medium' },
    { value: '3', viewValue: 'High' },
  ];
  // hack fix
  // we change _categories when `categoryService.getCategories().length` does not match the `_categories.length`
  // mapping `categoryService.getCategories()` in getter causes an infinite loop due to how angular material select works
  _categories: { value: string; viewValue: string }[] = [];
  get categories() {
    if (
      this._categories.length !== this.categoryService.getCategories().length
    ) {
      this._categories = this.categoryService.getCategories().map((cat) => {
        return {
          value: cat.id.toString(),
          viewValue: cat.name,
        };
      });
    }
    return this._categories;
  }
  _statuses: { value: string; viewValue: string }[] = [];
  get statuses() {
    if (
      this._statuses.length !== this.statusService.getStatuses().length
    ) {
      this._statuses = this.statusService.getStatuses().map((cat) => {
        return {
          value: cat.id.toString(),
          viewValue: cat.name,
        };
      });
    }
    return this._statuses;
  }

  constructor(
    private taskService: TaskService,
    private categoryService: CategoryService,
    private statusService: StatusService,
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router,
    private dialog: MatDialog
  ) {}

  async ngOnInit() {
    this.route.params.subscribe((params) => {
      this.projectId = parseInt(params['id']);
    });
    if (typeof this.projectId !== 'number') return;
    await this.taskService.fetchTasks({ projectId: this.projectId });
    this._categories = this.categoryService.getCategories().map((cat) => {
      return {
        value: cat.id.toString(),
        viewValue: cat.name,
      };
    });
    this.tasks = this.taskService
      .getTasks()
      .map((t) => ({ value: t.id.toString(), viewValue: t.title }));
    await this.userService.fetchUsers();
    this.users = this.userService
      .getUsers()
      .map((u) => ({ value: '1', viewValue: `${u.firstName} ${u.lastName}` }));
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
    if (this.startDate.value.getTime() > this.dueDate.value.getTime()) {
      this.errorMessage = 'Please enter valid start/due dates';
      return;
    }
    await this.taskService.createTask(
      {
        title: this.title,
        description: this.description,
        // date: this.dueDate.value,
        startDate: this.startDate.value,
        dueDate: this.dueDate.value,
        category: this.category,
        priority: this.priority,
        status: this.status,
        assignedTo: this.assignee,
        dependencies: this.dependencies,
      },
      1
    );
    this.router.navigateByUrl(`/project/${this.projectId}/tasks`);
  }
  createCategory() {
    this.dialog.open(CreateCategoryModalComponent);
  }
  createStatus() {
    this.dialog.open(CreateStatusModalComponent);
  }
}
