import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-kanban-task-card',
  standalone: true,
  imports: [],
  templateUrl: './kanban-task-card.component.html',
  styleUrl: './kanban-task-card.component.css'
})
export class KanbanTaskCardComponent {
  @Input() task!: Readonly<{
    title: string;
    priority: 'High' | 'Medium' | 'Low';
    category: string;
    status: 'Finished' | 'Active' | 'Past Due';
    date: Date;
    id: number;
  }>
}
