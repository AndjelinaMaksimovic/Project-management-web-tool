import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { MaterialModule } from '../../../material/material.module';
import { Task, TaskService } from '../../../services/task.service';
import { RouterModule } from '@angular/router';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-dependant-tasks-card',
  standalone: true,
  imports: [ MaterialModule, RouterModule, NgIf ],
  templateUrl: './dependant-tasks-card.component.html',
  styleUrl: './dependant-tasks-card.component.css'
})
export class DependantTasksCardComponent {
  @Input() dependant: {dependant: Task, type: number}[] | undefined
  @Input() taskId?: number

  constructor(private taskService: TaskService){}

  async removeDependency(task: {dependant: Task, type: number}){
    if(!this.taskId || !this.dependant)
      return

    await this.taskService.deleteDependency(this.taskId, task.dependant.id)
    // if(await this.taskService.deleteDependency(this.taskId, task.dependant.id)){
    //   this.dependant.splice(this.dependant.indexOf(task), 1)
    // }
  }

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
