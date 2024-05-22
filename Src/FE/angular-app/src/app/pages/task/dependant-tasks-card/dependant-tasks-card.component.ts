import { Component, Input } from '@angular/core';
import { MaterialModule } from '../../../material/material.module';
import { Task } from '../../../services/task.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-dependant-tasks-card',
  standalone: true,
  imports: [ MaterialModule, RouterModule ],
  templateUrl: './dependant-tasks-card.component.html',
  styleUrl: './dependant-tasks-card.component.css'
})
export class DependantTasksCardComponent {
  @Input() dependant: {dependant: Task, type: number}[] | undefined

  removeDependency(task: Task){}

  typeToString(type: number): string{
    switch (type) {
      default:
      case 1:
        return 'Start to start'
      case 2:
        return 'Start to end'
      case 3:
        return 'End to start'
      case 4:
        return 'End to end'
    }
  }
}
