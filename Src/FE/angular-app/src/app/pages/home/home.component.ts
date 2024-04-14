import { Component, Input } from '@angular/core';
import { TopnavComponent } from '../../components/topnav/topnav.component';
import { ProjectItemComponent } from '../../components/project-item/project-item.component';
import { NgIf } from '@angular/common';
import { ProjectService } from '../../services/project.service';
import { MatDialog } from '@angular/material/dialog';
import { NewProjectComponent } from '../new-project/new-project.component';
import { FiltersComponent } from '../../components/filters/filters.component';
import { Filter } from '../../components/filters/filters.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ TopnavComponent, ProjectItemComponent, NgIf, FiltersComponent, FormsModule ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})

export class HomeComponent {
  search: string = "";

  filters: Map<string, Filter> = new Map<string, Filter>([
    ["DueDateAfter", new Filter({ name: 'Start date', icon: 'fa-regular fa-calendar', type: 'date' })],
    ["DueDateBefore", new Filter({ name: 'Due date', icon: 'fa-solid fa-flag-checkered', type: 'date' })],
    // ["AssignedTo", new Filter({ name: 'Assigned to', icon: 'fa-solid fa-user', type: 'select', items: [ new Item({ value: "1", name: "Test" })]})],
  ]);

  isFilterOpen: boolean = false;

  constructor(private projectService: ProjectService, private dialogue: MatDialog) {}

  projects: any;

  async ngOnInit(){
    await this.projectService.fetchProjectsLocalStorage('archived_project_filters');
    this.projects = this.projectService.getProjects();
  }

  filterItems() {
    this.projects = this.projectService.getProjects().filter(project => project.title.toLowerCase().includes(this.search.toLocaleLowerCase()) || project.description.toLowerCase().includes(this.search.toLocaleLowerCase())); // TO DO - getArchievedProjects
  }

  async fetchProjectsFromLocalStorage() {
    await this.projectService.fetchProjectsLocalStorage('project_filters');
    this.projects = this.projectService.getProjects();
  }

  @Input() mostRecentAccordionVisible: boolean = true;
  @Input() starredProjectsAccordionVisible: boolean = true;
  @Input() allProjectsAccordionVisible: boolean = true;

  openFilters() {
    this.isFilterOpen = !this.isFilterOpen;
  }

  toggleMostRecentAccordion() {
    this.mostRecentAccordionVisible = !this.mostRecentAccordionVisible;
  }

  toggleStarredProjectsAccordion() {
    this.starredProjectsAccordionVisible = !this.starredProjectsAccordionVisible;
  }

  toggleAllProjectsAccordion() {
    this.allProjectsAccordionVisible = !this.allProjectsAccordionVisible;
  }

  newProjectPopUp(){
    this.dialogue.open(NewProjectComponent, { autoFocus: false })
  }
}
