import { Component, Input } from '@angular/core';
import { Task, TaskService } from '../../../services/task.service';
import { MaterialModule } from '../../../material/material.module';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ClearableInputComponent } from '../../../components/clearable-input/clearable-input.component';
import { ProjectService } from '../../../services/project.service';

@Component({
  selector: 'app-updatable-title',
  standalone: true,
  imports: [
    MaterialModule,
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    ClearableInputComponent,
  ],
  templateUrl: './updatable-title.component.html',
  styleUrl: './updatable-title.component.css'
})
export class UpdatableTitleComponent {
  @Input() title? = '';
  viewTitle = ''  // temp title for editing
  @Input() role?: any = {};
  @Input() id?: number = 0 // project / task ID
  @Input({required: true}) isProject: boolean = false
  // get title(){
  //   return this._title || this.task?.title || ""
  // }
  // set title(newTitle: string){
  //   this._title = newTitle;
  // }

  constructor(private taskService: TaskService, private projectService: ProjectService) {}

  updateTitle() {
    this.title = this.viewTitle
    if(this.isProject)
      this.projectService.updateProject({
        id: this.id,
        title: this.viewTitle,
      });
    else
      this.taskService.updateTask({
        id: this.id,
        title: this.viewTitle,
      });
  }
}

// import { Component } from '@angular/core';

// @Component({
//   selector: 'app-updatable-title',
//   standalone: true,
//   imports: [],
//   templateUrl: './updatable-title.component.html',
//   styleUrl: './updatable-title.component.css'
// })
// export class UpdatableTitleComponent {

// }
