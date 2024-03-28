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
  priority: string | null = null;

  priorities = [
    { value: 'Low', viewValue: 'Low' },
    { value: 'Medium', viewValue: 'Medium' },
    { value: 'High', viewValue: 'High' },
  ];

  constructor(
    private taskService: TaskService,
    public dialogRef: MatDialogRef<TaskCreationModalComponent>
  ) {}

  async createTask() {
    if (!this.title || !this.date || !this.priority || !this.description) {
      this.errorMessage = "Please provide all required fields";
      return;
    }
    await this.taskService.createTask(
      {
        title: this.title,
        description: this.description,
        date: new Date(this.date),
        category: 'Finance',
        priority: this.priority as 'Low' | 'High' | 'Medium',
        status: 'Active',
      },
      1
    );
    this.dialogRef.close();
  }
}

// import { Component } from '@angular/core';
// import { MaterialModule } from '../../material/material.module';
// import { AuthService } from '../../services/auth.service';
// import { FormsModule } from '@angular/forms';
// import { CommonModule } from '@angular/common';
// import { ActivatedRoute, Router } from '@angular/router';
// import { ClearableInputComponent } from '../../components/clearable-input/clearable-input.component';
// import { EmailFieldComponent } from '../../components/email-field/email-field.component';
// import { SelectComponent } from '../../components/select/select.component';
// import { RolesService } from '../../services/roles.service';
// @Component({
//   selector: 'app-invite-modal',
//   standalone: true,
//   imports: [MaterialModule, FormsModule, CommonModule, ClearableInputComponent, EmailFieldComponent, MaterialModule, SelectComponent],
//   templateUrl: './invite-modal.component.html',
//   styleUrl: './invite-modal.component.css'
// })
// export class InviteModalComponent {
//   /**
//    * placeholder for API values
//    */
//   roles: {value: string, viewValue: string}[] = [];
//   email: string = '';
//   firstName: string = '';
//   lastName: string = '';
//   role: string | null = null;
//   errorMessage: string | null = null;

//   constructor(
//     private authService: AuthService,
//     private rolesService: RolesService,
//     private router: Router,
//     private route: ActivatedRoute
//   ) {}
//   async ngOnInit(){
//     const roles = await this.rolesService.getAllRoles();
//     if(!roles) return;
//     this.roles = roles.map(role => ({value: role.id.toString(), viewValue: role.roleName}))
//   }
//   async register() {
//     // check email
//     if (this.email === undefined) {
//       this.errorMessage = 'no email passed';
//       return;
//     }
//     if (this.role === null) {
//       this.errorMessage = 'please select a role';
//       return;
//     }
//     const res = await this.authService.register(
//       this.email,
//       this.firstName,
//       this.lastName,
//       this.role,
//     );
//     // if registration fails return
//     if (!res) {
//       this.errorMessage = 'registration failed';
//       return;
//     }
//     // on successful registration, redirect to home?
//     this.router.navigate(['/login']);
//   }
// }
