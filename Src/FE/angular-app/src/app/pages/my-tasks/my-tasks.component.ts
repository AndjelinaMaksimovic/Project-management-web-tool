import { Component } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { ClearableInputComponent } from '../../components/clearable-input/clearable-input.component';

@Component({
  selector: 'app-my-tasks',
  standalone: true,
  imports: [MaterialModule, ClearableInputComponent],
  templateUrl: './my-tasks.component.html',
  styleUrl: './my-tasks.component.css',
})
export class MyTasksComponent {
  tasks = [
    { name: 'task1', category: 'finance', priority: 'low', status: 'active' },
    { name: 'task2', category: 'finance', priority: 'low', status: 'active' },
  ];
}
