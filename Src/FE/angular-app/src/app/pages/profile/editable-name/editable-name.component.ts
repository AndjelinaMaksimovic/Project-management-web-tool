import { Component, Input } from '@angular/core';
import { MaterialModule } from '../../../material/material.module';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ClearableInputComponent } from '../../../components/clearable-input/clearable-input.component';
import { UserService } from '../../../services/user.service';

@Component({
  selector: 'app-editable-name',
  standalone: true,
  imports: [
    MaterialModule,
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    ClearableInputComponent,
  ],
  templateUrl: './editable-name.component.html',
  styleUrl: './editable-name.component.css'
})
export class EditableNameComponent {
  @Input() user: any = undefined;
  _title: string | undefined = undefined;
  get title(){
    return this._title || this.user?.title || ""
  }
  set title(newTitle: string){
    this._title = newTitle;
  }

  constructor(private userService: UserService) {}

  updateName() {
    if (!this.user) return;
    // this.taskService.updateTask({
    //   id: this.user.id,
    //   title: this._title,
    // });
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

// import { Component } from '@angular/core';

// @Component({
//   selector: 'app-editable-name',
//   standalone: true,
//   imports: [],
//   templateUrl: './editable-name.component.html',
//   styleUrl: './editable-name.component.css'
// })
// export class EditableNameComponent {

// }
