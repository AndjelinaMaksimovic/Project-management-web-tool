import { Component } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { MaterialModule } from '../../material/material.module';
import { FormsModule } from '@angular/forms';
import { MatListModule } from '@angular/material/list';
import { MatDialog } from '@angular/material/dialog';
import { InvitePopupComponent } from '../../components/invite-popup/invite-popup.component';

@Component({
  selector: 'app-new-project',
  standalone: true,
  imports: [ NavbarComponent, MaterialModule, FormsModule, MatListModule ],
  templateUrl: './new-project.component.html',
  styleUrl: './new-project.component.css'
})
export class NewProjectComponent {
  constructor(private dialogue: MatDialog){}

  user = {
    name: "Petar",
    surname: "Petrovic"
  }

  projectName = ""
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
}
