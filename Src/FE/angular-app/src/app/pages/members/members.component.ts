import { Component, Input } from '@angular/core';
import { TopnavComponent } from '../../components/topnav/topnav.component';
import { FiltersComponent, Filter } from '../../components/filters/filters.component';
import { FormsModule } from '@angular/forms';
import { KeyValuePipe, NgFor, NgIf } from '@angular/common';
import { MemberItemComponent } from '../../components/member-item/member-item.component';
import { RolesService } from '../../services/roles.service';
import { UserService } from '../../services/user.service';
import { UserStatsComponent } from '../../components/user-stats/user-stats.component';
import { MatDialog } from '@angular/material/dialog';
import { AvatarService } from '../../services/avatar.service';
import { ActivatedRoute } from '@angular/router';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { ConfirmationDialogComponent } from '../../components/confirmation-dialog/confirmation-dialog.component';
import { NewMemberModalComponent } from '../../components/new-member-modal/new-member-modal.component';

class Member {
  firstname: string;
  lastname: string;
  id: number;
  image: string;
  projects: number;

  constructor(firstname: string, lastname: string, id: number, image: string, projects: number) {
    this.firstname = firstname;
    this.lastname = lastname;
    this.id = id;
    this.image = image;
    this.projects = projects;
  }

  getFullName() {
    return this.firstname + " " + this.lastname;
  }
}

class Role {
  name: string;
  id: number;
  _members: Array<Member>;
  members: Array<Member>;
  active: boolean = true;

  constructor(name: string, id: number, members: Array<Member>) {
    this.name = name;
    this.id = id;
    this._members = members;
    this.members = this._members;
  }

  addMember(member: Member) {
    this._members.push(member);
  }

  filterMembers(name: string) {
    this.members = this._members.filter((member) => member.getFullName().toLowerCase().includes(name.toLowerCase()));
    if(this.members.length == 0) {
      this.active = false;
    }
    else {
      this.active = true;
    }
  }
}

@Component({
  selector: 'app-members',
  standalone: true,
  imports: [ TopnavComponent, FormsModule, FiltersComponent, NgIf, NgFor, MemberItemComponent, KeyValuePipe, UserStatsComponent, NavbarComponent ],
  templateUrl: './members.component.html',
  styleUrl: './members.component.css'
})

export class MembersComponent {
  search: string = "";

  filters: Map<string, Filter> = new Map<string, Filter>([
    // ["DueDateAfter", new Filter({ name: 'Start date', icon: 'fa-regular fa-calendar', type: 'date' })],
    // ["DueDateBefore", new Filter({ name: 'Due date', icon: 'fa-solid fa-flag-checkered', type: 'date' })],
    // ["AssignedTo", new Filter({ name: 'Assigned to', icon: 'fa-solid fa-user', type: 'select', items: [ new Item({ value: "1", name: "Test" })]})],
  ]);

  roles: Map<number, Role> = new Map<number, Role>();

  isFilterOpen: boolean = false;

  constructor(private route: ActivatedRoute, private rolesService : RolesService, private userService: UserService, public dialog: MatDialog, private avatarService: AvatarService) {}

  filterRolesByName() {
    this.roles.forEach((role, key) => role.filterMembers(this.search));
  }

  isProject: boolean = false;
  projectId: number = -1;



  async ngOnInit() {
    this.route.data.subscribe(async(data) => {
      this.isProject = data['isProject'] || false;
      if(this.isProject) {
        await this.route.params.subscribe((params) => {
          this.projectId = parseInt(params['id']);
        });
      }
    });
    if(this.isProject) {
      // let onlyRoles = await this.rolesService.getProjectRoles(this.projectId);
      // onlyRoles?.forEach((val, index) => {
      //   this.roles.set(val.id, new Role(val.roleName, val.id, []));
      // });

      await this.userService.fetchUsersByProject(this.projectId);
      let onlyUsers = await this.userService.getUsers();
      console.log(onlyUsers);

      onlyUsers?.forEach((val, index) => {
        if(!this.roles.has(val.roleId)) {
          this.roles.set(val.roleId, new Role(val.roleName ? val.roleName : "Undefined", val.roleId, []));
        }
        this.roles.get(val.roleId ? val.roleId : -1)?.addMember(new Member(val.firstName, val.lastName, val.id, this.avatarService.getProfileImagePath(val.id), 0));
      });
    }
    else {
      let onlyRoles = await this.rolesService.getAllRoles();
      onlyRoles?.forEach((val, index) => {
        this.roles.set(val.id, new Role(val.roleName, val.id, []));
      });
      // this.roles.set(-1, new Role("No role", -1, []));

      await this.userService.fetchUsers();
      let onlyUsers = await this.userService.getUsers();

      onlyUsers?.forEach((val, index) => {
        this.roles.get(val.roleId ? val.roleId : -1)?.addMember(new Member(val.firstName, val.lastName, val.id, this.avatarService.getProfileImagePath(val.id), 0));
      });
    }
  }

  async fetchMembersFromLocalStorage() {

  }

  toggleAccordion(id: number) {
    let role = this.roles.get(id);
    if(role) {
      role.active = !role.active;
    }
  }

  openFilters() {
    this.isFilterOpen = !this.isFilterOpen;
  }

  openMember(id: number) {
    const dialogRef = this.dialog.open(UserStatsComponent, {
      panelClass: 'borderless-dialog',
      data: {
        id: id,
        title: !this.isProject ? "User details" : "User details on project",
        projectId: this.projectId
      },
      maxHeight: '90vh'
    });
  }

  removeUser(event: Event, member: Member) {
    event.stopPropagation();
    
    let descriptionMessage = "Are you sure you want to remove user <b>" + member.getFullName() + "</b> from the project?<br>This action cannot be undone and may affect project permissions and collaboration.";
    this.dialog.open(ConfirmationDialogComponent, { data: { title: "Confirm User Removal", description: descriptionMessage, yesFunc: async () => {
      await this.userService.removeUserFromProject(this.projectId, member.id);
    }, noFunc: () => { } } });
  }

  openNewMember() {
    this.dialog.open(NewMemberModalComponent, { autoFocus: false });
  }
}