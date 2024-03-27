import { Component, Input } from '@angular/core';
import { TopnavComponent } from '../../components/topnav/topnav.component';
import { ProjectItemComponent } from '../../components/project-item/project-item.component';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ TopnavComponent, ProjectItemComponent, NgIf ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
  @Input() mostRecentAccordionVisible: boolean = true;
  @Input() starredProjectsAccordionVisible: boolean = true;
  @Input() allProjectsAccordionVisible: boolean = true;

  toggleMostRecentAccordion() {
    this.mostRecentAccordionVisible = !this.mostRecentAccordionVisible;
  }

  toggleStarredProjectsAccordion() {
    this.starredProjectsAccordionVisible = !this.starredProjectsAccordionVisible;
  }

  toggleAllProjectsAccordion() {
    this.allProjectsAccordionVisible = !this.allProjectsAccordionVisible;
  }
}
