import { Component, EventEmitter, Input, Output } from '@angular/core';
import { SelectComponent } from '../../select/select.component';
import { MaterialModule } from '../../../material/material.module';
import { Task, TaskService } from '../../../services/task.service';

@Component({
  selector: 'app-add-dependant-tasks-chip',
  standalone: true,
  imports: [ MaterialModule, SelectComponent ],
  templateUrl: './add-dependant-tasks-chip.component.html',
  styleUrl: './add-dependant-tasks-chip.component.css'
})
export class AddDependantTasksChipComponent {
  @Input() currentTask: Task | undefined
  @Input() dependantTasks: {dependant: Task, type: number}[] | undefined
  @Output() dependantTasksOutput: any = new EventEmitter<any>()

  tasks: Task[] = []
  _chosenTask?: Task

  constructor(private taskService: TaskService){}

  async getDep(){
    if(!this.currentTask || !this.currentTask.projectId)
      return
    if(this.taskService.getTasks().length == 0)
      await this.taskService.fetchTasks({projectId: this.currentTask.projectId})
    this.tasks = this.taskService.getTasks().filter(allTasks =>  (allTasks.id != this.currentTask?.id) && (!this.dependantTasks?.find((depTask: any) => allTasks.id == depTask.dependant.id)))
  }
  chooseDep(task: Task){
    this._chosenTask = task
  }
  async addDep(type: number){
    if(!this.currentTask || !this._chosenTask)
      return

    await this.taskService.createTaskDependency({taskId: this.currentTask.id, dependentTaskId: this._chosenTask.id, typeOfDependencyId: type})
    // if(await this.taskService.createTaskDependency({taskId: this.currentTask.id, dependentTaskId: this._chosenTask.id, typeOfDependencyId: type})){
    //   this.dependantTasks?.push({dependant: this._chosenTask, type: type})
    //   this.dependantTasksOutput.emit(this.dependantTasks)
    // }
  }
}
