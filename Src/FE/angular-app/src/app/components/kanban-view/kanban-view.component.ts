import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-kanban-view',
  standalone: true,
  imports: [],
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
