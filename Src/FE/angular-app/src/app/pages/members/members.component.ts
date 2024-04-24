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
}

class Role {
  name: string;
  id: number;
  members: Array<Member>;
  active: boolean = true;

  constructor(name: string, id: number, members: Array<Member>) {
    this.name = name;
    this.id = id;
    this.members = members;
  }

  addMember(member: Member) {
    this.members.push(member);
  }
}

@Component({
  selector: 'app-members',
  standalone: true,
  imports: [ TopnavComponent, FormsModule, FiltersComponent, NgIf, NgFor, MemberItemComponent, KeyValuePipe, UserStatsComponent ],
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

  constructor(private rolesService : RolesService, private userService: UserService, public dialog: MatDialog) {}

  get _roles() {
    return this.roles;
  }

  async ngOnInit(){
    let onlyRoles = await this.rolesService.getAllRoles();
    onlyRoles?.forEach((val, index) => {
      this.roles.set(val.id, new Role(val.roleName, val.id, []));
    });
    this.roles.set(-1, new Role("No role", -1, []));

    await this.userService.fetchUsers();
    let onlyUsers = await this.userService.getUsers();

    onlyUsers?.forEach((val, index) => {
      this.roles.get(val.roleId ? val.roleId : -1)?.addMember(new Member(val.firstName, val.lastName, val.id, val.profilePicture, 0));
    });
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
        title: "User details"
      }
    });
  }
}