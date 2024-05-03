import { Component, Input } from '@angular/core';
import { TaskCardComponent } from '../task-card/task-card.component';
import { Task } from '../../services/task.service';
import { MaterialModule } from '../../material/material.module';

@Component({
  selector: 'app-tasks-table',
  standalone: true,
  imports: [TaskCardComponent, MaterialModule],
  templateUrl: './tasks-table.component.html',
  styleUrl: './tasks-table.component.css'
})
export class TasksTableComponent {
  @Input() tasks!: Task[];
}
