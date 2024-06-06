import { Component, Input } from '@angular/core';
import { TaskCardComponent } from '../task-card/task-card.component';
import { Task } from '../../services/task.service';
import { MaterialModule } from '../../material/material.module';
import { PageEvent } from '@angular/material/paginator';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-tasks-table',
  standalone: true,
  imports: [TaskCardComponent, MaterialModule, CommonModule],
  templateUrl: './tasks-table.component.html',
  styleUrl: './tasks-table.component.css'
})
export class TasksTableComponent {
  @Input() tasks!: Task[];
  @Input() role: any = {};
  columns = [
    {
      name: "Name", 
      sortFunction: (t1: Task, t2: Task) => {
        return t1.title.localeCompare(t2.title);
      }
    },
    {
      name: "Assignee", 
      sortFunction: (t1: Task, t2: Task) => {
        return t1.assignedTo?.length - t2.assignedTo?.length;
      }
    },
    {
      name: "Category", 
      sortFunction: (t1: Task, t2: Task) => {
        return t1.category.localeCompare(t2.category); 
      }
    },
    {
      name: "Priority", 
      sortFunction: (t1: Task, t2: Task) => {
        return t1.priority.localeCompare(t2.priority); 
      }
    },
    {
      name: "Status", 
      sortFunction: (t1: Task, t2: Task) => {
        return t1.status.localeCompare(t2.status); 
      }
    },
    {
      name: "Progress", 
      sortFunction: (t1: Task, t2: Task) => {
        return t1.progress - t2.progress; 
      }
    },
    {
      name: "Date", 
      sortFunction: (t1: Task, t2: Task) => {
        return t1.dueDate.getTime() - t2.dueDate.getTime(); 
      }
    },
  ]
  sortState: {name: string, mode: "asc" | "desc", sortFunction: ((t1: Task, t2: Task) => number)} = {
    name: "Name",
    mode: "desc",
    sortFunction: ((t1: Task, t2: Task) => 1)
  };
  toggleSortState(name: string){
    const mode = this.sortState.name === name ? this.sortState.mode === "asc" ? "desc" : "asc" : "desc"
    this.sortState = {
      name: name,
      mode: mode,
      sortFunction: (t1: Task, t2: Task) => (mode === "asc" ? -1 : 1) * this.columns.find(c => c.name! === name)!.sortFunction(t1, t2),
    }
  }
  get paginatedTasks(){
    const sorted = [...this.tasks].sort(this.sortState.sortFunction);
    return sorted.slice(this.pageIndex * this.pageSize, this.pageIndex * this.pageSize + this.pageSize)
  };

  pageIndex: number = 0;
  pageSize: number = 10;

  pageChangeEvent(event: PageEvent){
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
  }
}
