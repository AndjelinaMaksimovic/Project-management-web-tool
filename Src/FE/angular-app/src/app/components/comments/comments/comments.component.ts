import { Component } from '@angular/core';
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
  comments: Comment[] = [];
  constructor(private commentsService: CommentsService) {}
  ngOnInit(){
    this.comments = this.commentsService.getComments();
  }
}
