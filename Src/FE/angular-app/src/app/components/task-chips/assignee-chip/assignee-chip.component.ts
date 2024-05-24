import { Component, Input } from '@angular/core';
import { Task, TaskService } from '../../../services/task.service';
import { MaterialModule } from '../../../material/material.module';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SelectComponent } from '../../select/select.component';
import { UserService } from '../../../services/user.service';
import { AvatarStackComponent } from '../../avatar-stack/avatar-stack.component';

@Component({
  selector: 'app-assignee-chip',
  standalone: true,
  imports: [
    MaterialModule,
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    SelectComponent,
    AvatarStackComponent,
  ],
  templateUrl: './assignee-chip.component.html',
  styleUrl: './assignee-chip.component.css'
})
export class AssigneeChipComponent {
  @Input() task: Task | undefined;
  assignee: string | undefined;
  users: any;
  get asigneeIds(){
    return this.task?.assignedTo.map((user: any) => user.id) || [];
  }
  constructor(private taskService: TaskService, private userService: UserService) {}

  async ngOnInit(){
    await this.userService.fetchUsers();
    this.users = this.userService
    .getUsers()
    .map((u) => ({ value: u.id, viewValue: `${u.firstName} ${u.lastName}` }));
  }

  updateAssignees() {
    if (!this.task) return;
    this.taskService.updateTask({
      id: this.task.id,
      userId: this.assignee,
    });
  }
}


// import { Component } from '@angular/core';

// @Component({
//   selector: 'app-assignee-chip',
//   standalone: true,
//   imports: [],
//   templateUrl: './assignee-chip.component.html',
//   styleUrl: './assignee-chip.component.css'
// })
// export class AssigneeChipComponent {

// }
