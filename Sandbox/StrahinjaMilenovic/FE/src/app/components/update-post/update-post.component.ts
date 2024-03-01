import { Component, EventEmitter, Output, inject } from '@angular/core';
import { PostService } from '../../services/post.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-update-post',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './update-post.component.html',
  styleUrl: './update-post.component.css'
})
export class UpdatePostComponent {
  @Output() submit = new EventEmitter();
  postService = inject(PostService);
  postText: string = "";
  onClick(){
    this.submit.emit(this.postText);
  }
}
