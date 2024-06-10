import { AfterViewInit, Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
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
import { MatTabsModule } from '@angular/material/tabs';
import {Sort, MatSortModule, MatSort} from '@angular/material/sort';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatTableDataSource, MatTableModule } from '@angular/material/table'
import { merge, startWith, switchMap } from 'rxjs';

class Member{
  constructor(
    public id: number,
    public roleId: number,
    public name: string,
  ){}
}
class Permission{
  constructor(
    public name: string,
    public value: string,
    public selected: boolean,
  ){}
}
class Role{
  constructor(
    public id: number,
    public name: string,
    public permissions: Permission[]
  ){}
}

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
    MatTabsModule,
    MatSortModule,
    MatTableModule,
    MatPaginatorModule,
  ],
})
export class InviteToProjectModalComponent implements OnInit{
  constructor(
    private dialogue: MatDialog,
    private projectService: ProjectService,
    private dialogRef: MatDialogRef<InviteToProjectModalComponent>,
    private router: Router,
    private userService: UserService,
    private rolesService: RolesService,
    @Inject(MAT_DIALOG_DATA) private data: any
  ) {}

  members: Member[] = [];
  roles: Role[] = [];
  permissions: Permission[] = [];

  member?: Member;
  role?: Role;
  error = "";
  currentPermissions: Permission[] = [];
  newRoleName = new FormControl('', [Validators.required]);
  membersSelect: {value: Member, viewValue: string}[] = []

  columns: string[] = [];
  rolesData!: MatTableDataSource<any, MatPaginator>;

  selectedTabIndex = 0
  
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  async ngOnInit() {
    this.permissions = [
      {name: 'Add new user', value: 'CanAddNewUser', selected: true},
      {name: 'Invite user to project', value: 'CanAddUserToProject', selected: false},
      {name: 'Remove user from project', value: 'CanRemoveUserFromProject', selected: false},
      {name: 'Create new project', value: 'CanCreateProject', selected: false},
      {name: 'Delete project', value: 'CanDeleteProject', selected: true},
      {name: 'Update project', value: 'CanEditProject', selected: false},
      {name: 'Edit project', value: 'CanViewProject', selected: false},
      {name: 'View project', value: 'CanAddTaskToUser', selected: true},
      {name: 'Add task to user', value: 'CanCreateTask', selected: false},
      {name: 'Create task', value: 'CanRemoveTask', selected: false},
      {name: 'Edit task', value: 'CanEditTask', selected: false},
      {name: 'Edit user', value: 'CanEditUser', selected: false},
    ]
    this.currentPermissions = this.permissions.map(p => p)
    
    this.columns = ['name', ...this.permissions.map(p => p.value)]

    await this.initMembers()
    this.membersSelect = this.members.map(m => {return {value: m, viewValue: m.name}})
    
    this.roles = (await this.rolesService.getAllRoles())?.map((r) => new Role(r.id, r.roleName, r.permissions.map(p => {
      const thisP = this.permissions.find(thisP => thisP.value.toLowerCase() == p.name.toLowerCase())
      if(!thisP)
        throw 'Permission doesn\'t exist'
      return new Permission(thisP.name, thisP.value, p.value)
    }))) ?? [];
    
    this.role = this.getRoleFromId(this.members[0].roleId)

    this.initTable()
  }

  async initMembers(){
    await this.userService.fetchUsersByProject(this.data.projectId)
    const projectUsers = this.userService.getUsers()
    await this.userService.fetchUsers();
    const allUsers = this.userService.getUsers()
    const avaliableUsers = allUsers.filter(_allUsers => !projectUsers.find((projectUsers: any) => _allUsers.id == projectUsers.id))

    this.members = avaliableUsers.map((u) => {
      return new Member(u.id, u.roleId, `${u.firstName} ${u.lastName}`);
    });

    if(this.members.length == 0)
      throw "No members"

    this.member = this.members[0]
  }

  initTable(){
    this.rolesData = new MatTableDataSource<any, MatPaginator>(this.roles)
    this.rolesData.paginator = this.paginator
    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));
    this.rolesData.sort = this.sort
    this.rolesData.sortData = (data: any[], sort: MatSort) => {
      console.log(sort)
      return data.sort((a: any, b: any) => {
        if(sort.active && sort.active != 'name'){
          a = a.permissions.find((p: any) => p.value == sort.active).selected
          b = b.permissions.find((p: any) => p.value == sort.active).selected
        }
        return this.compare(a, b, sort.direction == 'asc')
      });
     }
  }
  
  getRoleFromId(id: number){
    const role = this.roles.find(r => r.id == id)
    if(role)
      return role
    throw "No role with id " + id
  }

  compare(a: number | string, b: number | string, isAsc: boolean) {
    return (a < b ? -1 : (a == b ? 0 : 1)) * (isAsc ? 1 : -1);
  }

  applyFilter(event: any){
    const filterValue = (event.target as HTMLInputElement).value;
    this.rolesData.filter = filterValue.trim().toLowerCase();

    if (this.rolesData.paginator) {
      this.rolesData.paginator.firstPage();
    }
  }
  
  selectRole(row: Role){
    this.role = row
    this.selectedTabIndex = 0
  }
  async invite(){
    if(!this.role || !this.member)
      return
    const success = await this.projectService.addNewUserToProject(this.data.projectId, this.member.id, this.role.id)
    // if(success)
      this.dialogRef.close(this.member.id)
    // else
      // this.error = "An error occured while inviting member"/
  }
  async createCustomRole(){
    this.newRoleName.markAsTouched()
    if(!this.newRoleName.value || this.newRoleName.hasError('required'))
      return

    const newRoleId = await this.rolesService.createCustomRole(this.newRoleName.value, this.permissions.filter(perm => perm.selected).map(perm => perm.value))
    if(newRoleId){
      const r = new Role(newRoleId, this.newRoleName.value, this.permissions.map(p => new Permission(p.name, p.value, p.selected)))
      this.roles.push(r)
      this.role = r
      this.selectedTabIndex = 0
    }
  }
}