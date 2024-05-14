import { Component, Input } from '@angular/core';
import { TopnavComponent } from '../../components/topnav/topnav.component';
import { ProjectItemComponent } from '../../components/project-item/project-item.component';
import { NgClass, NgIf } from '@angular/common';
import { ProjectService, Project } from '../../services/project.service';
import { MatDialog } from '@angular/material/dialog';
import { NewProjectModalComponent } from '../../components/new-project-modal/new-project-modal.component';
import { FiltersComponent } from '../../components/filters/filters.component';
import { Filter } from '../../components/filters/filters.component';
import { FormsModule } from '@angular/forms';
import { LocalStorageService } from '../../services/localstorage';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ TopnavComponent, ProjectItemComponent, NgIf, FiltersComponent, FormsModule, NgClass ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})

export class HomeComponent {
  search: string = "";

  // userId: number;

  filters: Map<string, Filter> = new Map<string, Filter>([
    ["DueDateAfter", new Filter({ name: 'Start date', icon: 'fa-regular fa-calendar', type: 'date' })],
    ["DueDateBefore", new Filter({ name: 'Due date', icon: 'fa-solid fa-flag-checkered', type: 'date' })],
    // ["AssignedTo", new Filter({ name: 'Assigned to', icon: 'fa-solid fa-user', type: 'select', items: [ new Item({ value: "1", name: "Test" })]})],
  ]);

  isFilterOpen: boolean = false;

  get activeFilters() {
    return Object.keys(this.localStorageService.getData("project_filters")).length;
  }

  constructor(private projectService: ProjectService, private dialogue: MatDialog, private localStorageService: LocalStorageService) {}

  get projects(){
    return this.projectService.getProjects().filter(project => project.title.toLowerCase().includes(this.search.toLocaleLowerCase()) || project.description.toLowerCase().includes(this.search.toLocaleLowerCase())).filter(project => !project.archived);
  }

  get archivedProjects(){
    return this.projectService.getProjects().filter(project => project.title.toLowerCase().includes(this.search.toLocaleLowerCase()) || project.description.toLowerCase().includes(this.search.toLocaleLowerCase())).filter(project => project.archived);
  }

  get starredProjects(){
    return this.projectService.getStarredProjects().filter(project => project.title.toLowerCase().includes(this.search.toLocaleLowerCase()) || project.description.toLowerCase().includes(this.search.toLocaleLowerCase())).filter(project => project.archived);
  }

  get projectProgress() {
    return this.projectService.getProgresses();
  }

  async ngOnInit(){
    await this.projectService.fetchProjectsLocalStorage('archived_project_filters');
    // await this.projectService.fetchStarredProjects(this.userId);
    // this.projects = this.projectService.getProjects().filter(project => !project.archived);

    if(this.starredProjects.length == 0) {
      this.staredProjectsAccordionVisible = false;
    }
    if(this.projects.length == 0) {
      this.activeProjectsAccordionVisible = false;
    }
  }

  filterItems() {
    // this.projects = this.projectService.getProjects().filter(project => project.title.toLowerCase().includes(this.search.toLocaleLowerCase()) || project.description.toLowerCase().includes(this.search.toLocaleLowerCase())).filter(project => !project.archived);
  }

  async fetchProjectsFromLocalStorage() {
    await this.projectService.fetchProjectsLocalStorage('project_filters');
    // this.projects = this.projectService.getProjects().filter(project => !project.archived);
  }

  staredProjectsAccordionVisible: boolean = true;
  activeProjectsAccordionVisible: boolean = true;
  archivedProjectsAccordionVisible: boolean = false;

  openFilters() {
    this.isFilterOpen = !this.isFilterOpen;
  }

  toggleStarred() {
    this.staredProjectsAccordionVisible = !this.staredProjectsAccordionVisible
  }

  toggleActive() {
    this.activeProjectsAccordionVisible = !this.activeProjectsAccordionVisible
  }
  
  toggleArchived() {
    this.archivedProjectsAccordionVisible = !this.archivedProjectsAccordionVisible
  }

  newProjectPopUp(){
    this.dialogue.open(NewProjectModalComponent, { autoFocus: false })
  }

  onFilterChange(data: boolean) {
    this.isFilterOpen = data;
  }
}
