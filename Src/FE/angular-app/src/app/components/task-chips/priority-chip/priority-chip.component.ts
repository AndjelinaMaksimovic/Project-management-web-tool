import { Component, Input } from '@angular/core';
import { Task, TaskService } from '../../../services/task.service';
import { MaterialModule } from '../../../material/material.module';
import { CategoryService } from '../../../services/category.service';

@Component({
  selector: 'app-priority-chip',
  standalone: true,
  imports: [MaterialModule],
  templateUrl: './priority-chip.component.html',
  styleUrl: './priority-chip.component.css',
})
export class PriorityChipComponent {
  @Input() task: Task | undefined;

  constructor(
    private categoryService: CategoryService,
    private taskService: TaskService
  ) {}

  get categorieNames() {
    return this.categoryService.getCategories().map((c) => c.name);
  }

  get categories() {
    return this.categoryService.getCategories();
  }

  updateCategory(categoryId: string) {
    console.log(categoryId);
    if (!this.task) return;
    this.taskService.updateTask({
      id: this.task.id,
      categoryId: categoryId,
    });
  }
}
