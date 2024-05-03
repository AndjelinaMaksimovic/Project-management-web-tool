import { Component, Input } from '@angular/core';
import { TaskCardComponent } from '../task-card/task-card.component';
import { Task } from '../../services/task.service';
import { MaterialModule } from '../../material/material.module';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-tasks-table',
  standalone: true,
  imports: [TaskCardComponent, MaterialModule],
  templateUrl: './tasks-table.component.html',
  styleUrl: './tasks-table.component.css'
})
export class TasksTableComponent {
  @Input() tasks!: Task[];
  @Input() paginatedTasks!: Task[];

  pageIndex: number = 0;
  pageSize: number = 10;

  pageChangeEvent(event: PageEvent){
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    const pageIndex = event.pageIndex;
    const pageSize = event.pageSize;
    this.paginatedTasks = this.tasks.slice(pageIndex * pageSize, pageIndex * pageSize + pageSize)
    // this.getPagedData();
  }
}
