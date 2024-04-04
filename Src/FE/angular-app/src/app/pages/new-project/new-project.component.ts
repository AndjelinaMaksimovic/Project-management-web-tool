import { Component } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatListModule } from '@angular/material/list';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { InvitePopupComponent } from '../../components/invite-popup/invite-popup.component';
import { TopnavComponent } from "../../components/topnav/topnav.component";
import { MAT_DATE_LOCALE, provideNativeDateAdapter } from '@angular/material/core';
import { ProjectService } from '../../services/project.service';
import { MatIconModule } from '@angular/material/icon';
import { NgIf } from '@angular/common';

@Component({
    selector: 'app-new-project',
    standalone: true,
    templateUrl: './new-project.component.html',
    styleUrl: './new-project.component.css',
    imports: [MaterialModule, FormsModule, ReactiveFormsModule, MatListModule, TopnavComponent, MatDatepickerModule, MatIconModule, NgIf ],
    providers: [ provideNativeDateAdapter(), { provide: MAT_DATE_LOCALE, useValue: 'en-GB' } ]
})
export class NewProjectComponent {
  constructor(private dialogue: MatDialog, private projectService: ProjectService, private dialogRef: MatDialogRef<NewProjectComponent>){}

  errorMessage = ""

  user = {
    name: "Petar",
    surname: "Petrovic"
  }

  projectName = ""
  description = ""
  usersSelected = []
  dueDate = new FormControl(new Date())
  projects = [
    'pr1',
    'project 2'
  ]

  users = [
    {
      name: 'Milos',
      role: 'Project Manager'
    },
    {
      name: 'Ana',
      role: 'Developer'
    }
  ]
  parentProject?: string

  invite(){
    this.dialogue.open(InvitePopupComponent, { autoFocus: false })
  }

  // Add new popup, povezi, na dugme zatvara
  // na home dodaj delete dugme
  // logout

  async createNewProject(){
    const res = await this.projectService.createNew({
      name: this.projectName,
      description: this.description,
      dueDate: this.dueDate.value,
      userIds: this.usersSelected
    })
    if(!res){
      this.errorMessage = 'Error creating project';
    }else{
      this.dialogRef.close()
    }
  }
}
