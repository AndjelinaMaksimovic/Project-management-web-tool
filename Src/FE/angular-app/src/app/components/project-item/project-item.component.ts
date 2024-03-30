import { Component, Input } from '@angular/core';
import { ProgressbarComponent } from '../progressbar/progressbar.component';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-project-item',
  standalone: true,
  imports: [ ProgressbarComponent, NgIf ],
  templateUrl: './project-item.component.html',
  styleUrl: './project-item.component.css'
})
export class ProjectItemComponent {
  @Input() projectName: string = "";
  @Input() dueDate: string = "";

  @Input() progressBarProgress: Number = 0;
  @Input() progressBarColor: string = "black";

  @Input() starred: boolean = false;
  @Input() id: number = 0;

  isHovered = false;

  toggleStarred() {
    this.starred = !this.starred;
  }

  toggleHovered() {
    this.isHovered = !this.isHovered;
  }
}
