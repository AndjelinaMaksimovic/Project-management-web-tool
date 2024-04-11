import { Component, Input } from '@angular/core';
import { Comment } from '../../../services/comments.service';
import { MaterialModule } from '../../../material/material.module';

@Component({
  selector: 'app-comment',
  standalone: true,
  imports: [MaterialModule],
  templateUrl: './comment.component.html',
  styleUrl: './comment.component.css'
})
export class CommentComponent {
  @Input()
  comment: Comment | undefined;
}
