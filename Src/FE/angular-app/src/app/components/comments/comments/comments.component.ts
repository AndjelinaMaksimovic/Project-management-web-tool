import { Component, Input } from '@angular/core';
import { Comment, CommentsService } from '../../../services/comments.service';
import { CommentComponent } from '../comment/comment.component';

@Component({
  selector: 'app-comments',
  standalone: true,
  imports: [CommentComponent],
  templateUrl: './comments.component.html',
  styleUrl: './comments.component.css'
})
export class CommentsComponent {
  @Input() taskId: number | undefined;
  get comments(){
    return this.commentsService.getComments();
  }
  constructor(private commentsService: CommentsService) {}
  ngOnInit(){
    this.commentsService.fetchComments({taskId: this.taskId});
  }
}
