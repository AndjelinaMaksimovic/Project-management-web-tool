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
  @Input() role: any = {};
  get paginatedTasks(){
    return this.tasks.slice(this.pageIndex * this.pageSize, this.pageIndex * this.pageSize + this.pageSize)
  };

  pageIndex: number = 0;
  pageSize: number = 10;

  pageChangeEvent(event: PageEvent){
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
  }
}
