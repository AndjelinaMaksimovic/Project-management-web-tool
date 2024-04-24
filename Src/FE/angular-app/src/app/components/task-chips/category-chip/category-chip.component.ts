import { Component, Input } from '@angular/core';
import { Task } from '../../../services/task.service';

@Component({
  selector: 'app-category-chip',
  standalone: true,
  imports: [],
  templateUrl: './category-chip.component.html',
  styleUrl: './category-chip.component.css'
})
export class CategoryChipComponent {
  @Input() task: Task | undefined;
}
