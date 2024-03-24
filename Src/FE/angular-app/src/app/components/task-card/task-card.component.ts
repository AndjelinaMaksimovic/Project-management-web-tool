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
  @Input() priority: ("High" | "Medium" | "Low") = "Medium";
  @Input() category: string = "category";
  @Input() status: ("Finished" | "Active" | "Past Due") = "Finished";
  @Input() date: string = "date";

  get priorityColor(){
    const priorityColorMap = {
      "High": "warn",
      "Medium": "mid",
      "Low": "neutral",
    } as const;
    return priorityColorMap[this.priority];
  }
  get statuspriorityColor(){
    const priorityColorMap = {
      "Finished": "good",
      "Active": "mid",
      "Past Due": "warn",
    } as const;
    return priorityColorMap[this.status];
  }
}
