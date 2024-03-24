import { Component, Input } from '@angular/core';
import { MaterialModule } from '../../material/material.module';

@Component({
  selector: 'app-task-card',
  standalone: true,
  imports: [MaterialModule],
  templateUrl: './task-card.component.html',
  styleUrl: './task-card.component.css',
})
export class TaskCardComponent {
  @Input() name: string = "name";
  @Input() priority: string = "priority";
  @Input() category: string = "category";
  @Input() status: string = "status";
  @Input() date: string = "date";
}
