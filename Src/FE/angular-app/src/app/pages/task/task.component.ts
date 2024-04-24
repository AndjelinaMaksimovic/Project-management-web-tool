import { Component } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { ActivatedRoute } from '@angular/router';
import { Task, TaskService } from '../../services/task.service';
import { MarkdownModule, provideMarkdown } from 'ngx-markdown';
import { MaterialModule } from '../../material/material.module';
import { CommentsComponent } from '../../components/comments/comments/comments.component';
import { EditableMarkdownComponent } from '../../components/editable-markdown/editable-markdown.component';
@Component({
  selector: 'app-task',
  standalone: true,
  imports: [NavbarComponent, MarkdownModule, MaterialModule, CommentsComponent, EditableMarkdownComponent],
  providers: [provideMarkdown()],
  templateUrl: './task.component.html',
  styleUrl: './task.component.css',
})
export class TaskComponent {
  taskId: number = 0;
  projectId: number = 0;
  task: Task | undefined;
  constructor(
    private taskService: TaskService,
    private route: ActivatedRoute
  ) {}

  async ngOnInit() {
    this.route.params.subscribe((params) => {
      this.taskId = parseInt(params['taskId']);
      this.projectId = parseInt(params['projectId']);
    });
    await this.taskService.fetchTasks({projectId: this.projectId});
    this.task = this.taskService.getTasks().find((t) => t.id === this.taskId);
  }

  updateDescription(newDescription: string){
    this.taskService.updateTask({
      id: this.taskId,
      description: newDescription
    })
  }
}
