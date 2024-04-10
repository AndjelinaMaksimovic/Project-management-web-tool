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
import { MatDialogRef } from '@angular/material/dialog';
import { CategoryService } from '../../services/category.service';
import {
  MAT_DATE_LOCALE,
  provideNativeDateAdapter,
} from '@angular/material/core';
import { UserService } from '../../services/user.service';

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
  category: string | null = null;
  dependencies: string[] = [];
  assignee: string | undefined;

  tasks: {value: string, viewValue: string}[] = [];
  users: {value: string, viewValue: string}[] = [];

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
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router,
  ) {
  }

  async ngOnInit() {
    this.route.params.subscribe((params) => {
      this.projectId = parseInt(params['id']);
    });
    if(typeof this.projectId !== "number") return;
    await this.taskService.fetchTasks({ projectId: this.projectId });
    this.categories = this.categoryService.getCategories().map((cat) => {
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
    .map((u) => ({ value: "1", viewValue: `${u.firstName} ${u.lastName}` }));
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
    if(this.startDate.value.getTime() > this.dueDate.value.getTime()){
      this.errorMessage = 'Please enter valid start/due dates';
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
    this.router.navigateByUrl(`/project/${this.projectId}/tasks`);
  }
}