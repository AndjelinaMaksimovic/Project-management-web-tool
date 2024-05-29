import { Component, Input } from '@angular/core';
import { ProgressbarComponent } from '../progressbar/progressbar.component';
import { NgIf } from '@angular/common';
import { ProjectService } from '../../services/project.service';
import { RouterModule } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
    selector: 'app-project-item',
    standalone: true,
    imports: [ProgressbarComponent, NgIf, RouterModule, DatePipe, MatTooltipModule ],
    templateUrl: './project-item.component.html',
    styleUrl: './project-item.component.css'
})
export class ProjectItemComponent {
    constructor(private projectService: ProjectService) { }

    @Input() projectName: string = "";
    @Input() dueDate: string = "";

    @Input() progressBarProgress: number = 0;
    @Input() progressBarColor: string = "black";

    @Input() starred: boolean = false;
    @Input() id: number = 0;

    @Input() isArchived: boolean = false;
    @Input() role: any = {};

    @Input() dontRefresh?: boolean = false;

    async toggleStarred() {
        let response = await this.projectService.toggleStarred(this.id, this.dontRefresh ? true : false);
        if(response) {
            this.starred = !this.starred;
        }
    }

    archiveProject() {
        this.projectService.archiveProject(this.id, this.dontRefresh ? true : false);
    }
}
