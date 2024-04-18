import { Component, Input } from '@angular/core';
import { Comment, CommentsService } from '../../../services/comments.service';
import { CommentComponent } from '../comment/comment.component';
import { MaterialModule } from '../../../material/material.module';

@Component({
  selector: 'app-comments',
  standalone: true,
  imports: [CommentComponent, MaterialModule],
  templateUrl: './comments.component.html',
  styleUrl: './comments.component.css'
})
export class CommentsComponent {
  newComment: string = "";
  @Input() taskId: number | undefined;
  get comments(){
    return this.commentsService.getComments();
  }
  constructor(private commentsService: CommentsService) {}
  ngOnInit(){
    this.commentsService.fetchComments({taskId: this.taskId});
  }
}
