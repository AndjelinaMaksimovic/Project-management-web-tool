import { Component } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { ActivatedRoute } from '@angular/router';
import { Task, TaskService } from '../../services/task.service';
import { MarkdownModule, provideMarkdown } from 'ngx-markdown';
import { MaterialModule } from '../../material/material.module';
@Component({
  selector: 'app-task',
  standalone: true,
  imports: [NavbarComponent, MarkdownModule, MaterialModule],
  providers: [provideMarkdown()],
  templateUrl: './task.component.html',
  styleUrl: './task.component.css',
})
export class TaskComponent {
  taskId: number = 0;
  task: Task | undefined;
  constructor(
    private taskService: TaskService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.params.subscribe((params) => {
      this.taskId = parseInt(params['id']);
    });
    this.task = this.taskService.getTasks().find((t) => t.id === this.taskId);
  }
}
