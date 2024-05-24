import { Component, Inject } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatListModule } from '@angular/material/list';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { InvitePopupComponent } from '../../components/invite-popup/invite-popup.component';
import { TopnavComponent } from '../../components/topnav/topnav.component';
import { SelectComponent } from '../../components/select/select.component';
import { ProjectService } from '../../services/project.service';
import { MatIconModule } from '@angular/material/icon';
import { NgIf } from '@angular/common';
import { MarkdownEditorComponent } from '../markdown-editor/markdown-editor.component';
import moment from 'moment';
import { ActivatedRoute, Router } from '@angular/router';
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
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  public member: any;
  public members: any[] = [];
  public roleId: number = 1;
  public roles: any[] = [];
  error = "";
  async ngOnInit() {
    await this.userService.fetchUsersByProject(this.data.projectId)
    const projectUsers = this.userService.getUsers()
    await this.userService.fetchUsers();
    const allUsers = this.userService.getUsers()
    const avaliableUsers = allUsers.filter(_allUsers => !projectUsers.find((projectUsers: any) => _allUsers.id == projectUsers.id))
    this.members = avaliableUsers.map((u) => {
      return { value: {id: u.id, roleId: u.roleId}, viewValue: `${u.firstName} ${u.lastName}` };
    });
    this.member = this.members[0]
    this.roles = (await this.rolesService.getAllRoles())?.map((r) => ({
      value: r.id,
      viewValue: r.roleName,
    })) ?? [];
    this.roleId = this.members[0].roleId
  }

  async invite(){
    const success = await this.projectService.addNewUserToProject(this.data.projectId, this.member.id, this.roleId)
    // if(success)
      this.dialogRef.close(true)
    // else
      // this.error = "An error occured while inviting member"/
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
