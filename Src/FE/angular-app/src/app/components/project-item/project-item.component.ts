import { Component, Input } from '@angular/core';
import { ProgressbarComponent } from '../progressbar/progressbar.component';
import { NgIf } from '@angular/common';
import { ProjectService } from '../../services/project.service';
import { RouterModule } from '@angular/router';
import { DatePipe } from '@angular/common';

@Component({
    selector: 'app-project-item',
    standalone: true,
    imports: [ProgressbarComponent, NgIf, RouterModule, DatePipe ],
    templateUrl: './project-item.component.html',
    styleUrl: './project-item.component.css'
})
export class ProjectItemComponent {
    constructor(private projectService: ProjectService) { }

    @Input() projectName: string = "";
    @Input() dueDate: string = "";

    @Input() progressBarProgress: Number = 0;
    @Input() progressBarColor: string = "black";

    @Input() starred: boolean = false;
    @Input() id: number = 0;

    toggleStarred() {
        this.starred = !this.starred;
    }

    archiveProject() {
        this.projectService.archiveProject(this.id);
    }
}
