import { Component, Input } from '@angular/core';
import { KanbanTaskCardComponent } from '../kanban-task-card/kanban-task-card.component';

@Component({
  selector: 'app-kanban-view',
  standalone: true,
  imports: [KanbanTaskCardComponent],
  templateUrl: './kanban-view.component.html',
  styleUrl: './kanban-view.component.css'
})
export class KanbanViewComponent {
  @Input() tasks!: Readonly<{
    title: string;
    priority: 'High' | 'Medium' | 'Low';
    category: string;
    status: 'Finished' | 'Active' | 'Past Due';
    date: Date;
    id: number;
  }[]>;

  statuses = ["Active", "In Progress", "Pending", "Completed"];
}
