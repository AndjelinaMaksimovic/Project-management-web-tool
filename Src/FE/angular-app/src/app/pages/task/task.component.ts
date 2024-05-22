import { Component } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { ActivatedRoute } from '@angular/router';
import { Task, TaskService } from '../../services/task.service';
import { MarkdownModule, provideMarkdown } from 'ngx-markdown';
import { MaterialModule } from '../../material/material.module';
import { CommentsComponent } from '../../components/comments/comments/comments.component';
import { EditableMarkdownComponent } from '../../components/editable-markdown/editable-markdown.component';
import { CategoryChipComponent } from '../../components/task-chips/category-chip/category-chip.component';
import { StatusChipComponent } from '../../components/task-chips/status-chip/status-chip.component';
import { DateChipComponent } from '../../components/task-chips/date-chip/date-chip.component';
import { PriorityChipComponent } from '../../components/task-chips/priority-chip/priority-chip.component';
import { UpdatableTitleComponent } from './updatable-title/updatable-title.component';
import { AssigneeChipComponent } from '../../components/task-chips/assignee-chip/assignee-chip.component';
import { ProgressChipComponent } from '../../components/task-chips/progress-chip/progress-chip.component';
@Component({
  selector: 'app-task',
  standalone: true,
  imports: [
    NavbarComponent,
    MarkdownModule,
    MaterialModule,
    CommentsComponent,
    EditableMarkdownComponent,
    CategoryChipComponent,
    StatusChipComponent,
    PriorityChipComponent,
    DateChipComponent,
    UpdatableTitleComponent,
    AssigneeChipComponent,
    ProgressChipComponent,
  ],
  providers: [provideMarkdown()],
  templateUrl: './task.component.html',
  styleUrl: './task.component.css',
})
export class TaskComponent {
  taskId: number = 0;
  projectId: number = 0;
  constructor(
    private taskService: TaskService,
    private route: ActivatedRoute
  ) {}

  get task() {
    return this.taskService.getTasks().find((t) => t.id === this.taskId);
  }

  async ngOnInit() {
    this.route.params.subscribe((params) => {
      this.taskId = parseInt(params['taskId']);
      this.projectId = parseInt(params['id']);
    });
    await this.taskService.fetchTasks({ projectId: this.projectId });
  }

  updateDescription(newDescription: string) {
    this.taskService.updateTask({
      id: this.taskId,
      description: newDescription,
    });
  }
}
