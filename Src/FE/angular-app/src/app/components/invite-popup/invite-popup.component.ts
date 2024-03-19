import { Component } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MaterialModule } from '../../material/material.module';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatListModule } from '@angular/material/list';

@Component({
  selector: 'app-invite-popup',
  standalone: true,
  imports: [ MatIconModule, MaterialModule, MatCheckboxModule, MatListModule ],
  templateUrl: './invite-popup.component.html',
  styleUrl: './invite-popup.component.css'
})
export class InvitePopupComponent {
  member?: string
  role?: {name: string, permissions: Array<string>}

  members = [
    'Milos Milovic',
    'Ivan Ivanovic'
  ]
  roles = [
    {
      name: 'Developer',
      permissions: ['Update project', 'Update task']
    },
    {
      name: 'Project manager',
      permissions: [
        'Invite user to project',
        'Remove user from project',
        'Update project',
        'Update task',
      ]
    },
    {
      name: 'Spectator',
      permissions: []
    },
  ]

  permissions = [
    'Add new user',
    'Invite user to project',
    'Remove user from project',
    'Create new project',
    'Update project',
    'Update task',
    'Delete project'
  ]

  invite(){
    
  }
}
