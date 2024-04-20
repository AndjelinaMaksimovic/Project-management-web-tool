import { Component, Input } from '@angular/core';
import { TopnavComponent } from '../../components/topnav/topnav.component';
import { FiltersComponent, Filter } from '../../components/filters/filters.component';
import { FormsModule } from '@angular/forms';
import { KeyValuePipe, NgFor, NgIf } from '@angular/common';
import { MemberItemComponent } from '../../components/member-item/member-item.component';

class Member {
  name: string;
  id: number;
  image: string;
  projects: number;

  constructor(name: string, id: number, image: string, projects: number) {
    this.name = name;
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
}

@Component({
  selector: 'app-members',
  standalone: true,
  imports: [ TopnavComponent, FormsModule, FiltersComponent, NgIf, NgFor, MemberItemComponent, KeyValuePipe ],
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

  constructor() {}

  get _roles() {
    return this.roles;
  }

  async ngOnInit(){
    // this.roles.set(1, new Role("Project owner", 2, [ new Member("Test", 1, "https://img.freepik.com/free-photo/handsome-bearded-guy-posing-against-white-wall_273609-20597.jpg?size=626&ext=jpg&ga=GA1.1.1224184972.1713571200&semt=sph", 0), new Member("Test", 1, "", 2) ]));
    // this.roles.set(2, new Role("Project manager", 2, [ new Member("Test", 1, "", 0)]));
    // this.roles.set(3, new Role("Employee", 2, [ new Member("Test", 1, "", 0)]));
    // this.roles.set(4, new Role("Viewer", 2, [ new Member("Test", 1, "", 0)]));
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
}