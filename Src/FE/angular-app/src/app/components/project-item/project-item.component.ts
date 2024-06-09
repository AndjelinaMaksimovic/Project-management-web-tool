import { Component, Input } from '@angular/core';
import { ProgressbarComponent } from '../progressbar/progressbar.component';
import { NgIf } from '@angular/common';
import { ProjectService } from '../../services/project.service';
import { RouterModule } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TaskService } from '../../services/task.service';
import { BehaviorSubject } from 'rxjs';

@Component({
    selector: 'app-project-item',
    standalone: true,
    imports: [ProgressbarComponent, NgIf, RouterModule, DatePipe, MatTooltipModule ],
    templateUrl: './project-item.component.html',
    styleUrl: './project-item.component.css'
})
export class ProjectItemComponent {
    constructor(private projectService: ProjectService, private taskService: TaskService) {
        this.progressBarProgress$.subscribe(value => {
            this.onProgressChange(value);
        });
    }

    private progressSubject = new BehaviorSubject<number>(0);

    @Input() projectName: string = "";
    @Input() dueDate: string = "";

    progressBarProgress$ = this.progressSubject.asObservable();
    @Input() progressBarColor: string = "black";

    @Input() starred: boolean = false;
    @Input() id: number = 0;

    @Input() isArchived: boolean = false;
    @Input() role: any = {};

    @Input() dontRefresh?: boolean = false;

    overdueTasks: number = 0;

    @Input() set progressBarProgress(value: number) {
        this.progressSubject.next(value);
    }
    
    get progressBarProgress(): number {
    return this.progressSubject.value;
    }
    
    onProgressChange(value: number) {
        this.progressBarColor = (this.progressBarProgress >= 100.0 ? "#00c20c" : (this.overdueTasks > 0 ? "#FF5733" : "#FFCF32" ));
    }

    async ngOnInit() {
        await this.taskService.fetchTasks({ projectId: this.id });
        this.overdueTasks = this.taskService.getTasks().filter((task) => new Date(task.dueDate) < new Date()).length;

        this.progressBarColor = (this.progressBarProgress >= 100.0 ? "#00c20c" : (this.overdueTasks > 0 ? "#FF5733" : "#FFCF32" ));
    }

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
