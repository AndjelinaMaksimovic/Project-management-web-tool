import { Component, Input } from '@angular/core';
import { Comment, CommentsService } from '../../../services/comments.service';
import { CommentComponent } from '../comment/comment.component';
import { MaterialModule } from '../../../material/material.module';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MarkdownEditorComponent } from '../../markdown-editor/markdown-editor.component';

@Component({
  selector: 'app-comments',
  standalone: true,
  imports: [CommentComponent, MaterialModule, FormsModule, MarkdownEditorComponent],
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
  async postComment(){
    await this.commentsService.postComment(this.newComment);
    this.newComment = "";
  }
}
