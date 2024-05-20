import { Component, Input } from '@angular/core';
import { Comment } from '../../../services/comments.service';
import { MaterialModule } from '../../../material/material.module';
import { MarkdownModule, provideMarkdown } from 'ngx-markdown';
import { AvatarService } from '../../../services/avatar.service';

@Component({
  selector: 'app-comment',
  standalone: true,
  imports: [MaterialModule, MarkdownModule],
  providers: [provideMarkdown()],
  templateUrl: './comment.component.html',
  styleUrl: './comment.component.css'
})
export class CommentComponent {
  @Input()
  comment: Comment | undefined;
  
  constructor(public avatarService: AvatarService) {}
}
