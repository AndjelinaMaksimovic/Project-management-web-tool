import { Component } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatListModule } from '@angular/material/list';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { InvitePopupComponent } from '../../components/invite-popup/invite-popup.component';
import { TopnavComponent } from '../../components/topnav/topnav.component';
import { SelectComponent } from '../../components/select/select.component';
import { ProjectService } from '../../services/project.service';
import { MatIconModule } from '@angular/material/icon';
import { NgIf } from '@angular/common';
import { MarkdownEditorComponent } from '../markdown-editor/markdown-editor.component';
import moment from 'moment';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { RolesService } from '../../services/roles.service';

@Component({
  selector: 'app-invite-to-project-modal',
  standalone: true,
  templateUrl: './invite-to-project-modal.component.html',
  styleUrl: './invite-to-project-modal.component.css',
  imports: [
    MarkdownEditorComponent,
    MaterialModule,
    FormsModule,
    ReactiveFormsModule,
    MatListModule,
    TopnavComponent,
    MatDatepickerModule,
    MatIconModule,
    NgIf,
    SelectComponent,
  ],
})
export class InviteToProjectModalComponent {
  constructor(
    private dialogue: MatDialog,
    private projectService: ProjectService,
    private dialogRef: MatDialogRef<InviteToProjectModalComponent>,
    private router: Router,
    private userService: UserService,
    private rolesService: RolesService,
  ) {}

  public memberId: string = "1";
  public members: {value: string, viewValue: string}[] = [];
  public roleId: string = "1";
  public roles: {value: string, viewValue: string}[] = [];
  async ngOnInit() {
    await this.userService.fetchUsers();
    this.members = this.userService.getUsers().map((u) => {
      return { value: u.id.toString(), viewValue: `${u.firstName} ${u.lastName}` };
    });
    this.roles = (await this.rolesService.getAllRoles())?.map((r) => ({
      value: r.id.toString(),
      viewValue: r.roleName,
    })) ?? [];
  }

  async invite(){
    try{
      this.projectService.addUserToProject(this.memberId, "1", this.roleId)
    } catch(e){
      console.log(e);
    }
  }
}

// import { Component } from '@angular/core';

// @Component({
//   selector: 'app-invite-to-project-modal',
//   standalone: true,
//   imports: [],
//   templateUrl: './invite-to-project-modal.component.html',
//   styleUrl: './invite-to-project-modal.component.css'
// })
// export class InviteToProjectModalComponent {

// }
