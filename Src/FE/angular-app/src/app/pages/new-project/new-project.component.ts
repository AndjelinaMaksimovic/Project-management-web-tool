import { Component } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatListModule } from '@angular/material/list';
import { MatDialog } from '@angular/material/dialog';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { InvitePopupComponent } from '../../components/invite-popup/invite-popup.component';
import { TopnavComponent } from "../../components/topnav/topnav.component";
import { provideNativeDateAdapter } from '@angular/material/core';
import { ProjectsService } from '../../services/projects.service';

@Component({
    selector: 'app-new-project',
    standalone: true,
    templateUrl: './new-project.component.html',
    styleUrl: './new-project.component.css',
    imports: [MaterialModule, FormsModule, ReactiveFormsModule, MatListModule, TopnavComponent, MatDatepickerModule ],
    providers: [ provideNativeDateAdapter() ]
})
export class NewProjectComponent {
  constructor(private dialogue: MatDialog, private projectService: ProjectsService){}

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

  createNewProject(){
    // this.projectService.createNew({name: this.projectName, descripion: this.description, dueDate: this.dueDate.value, userIds: this.usersSelected})
  }
}
