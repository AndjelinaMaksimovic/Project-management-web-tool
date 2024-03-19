import { Component } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { MaterialModule } from '../../material/material.module';
import { FormsModule } from '@angular/forms';
import { MatListModule } from '@angular/material/list';

@Component({
  selector: 'app-new-project',
  standalone: true,
  imports: [ NavbarComponent, MaterialModule, FormsModule, MatListModule ],
  templateUrl: './new-project.component.html',
  styleUrl: './new-project.component.css'
})
export class NewProjectComponent {
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
}
