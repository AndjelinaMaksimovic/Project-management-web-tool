import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { PostService } from '../../services/post.service';
import { CreatePostComponent } from '../create-post/create-post.component';

@Component({
  selector: 'app-posts',
  standalone: true,
  imports: [CommonModule, RouterModule, CreatePostComponent],
  templateUrl: './posts.component.html',
  styleUrl: './posts.component.css'
})
export class PostsComponent {
  posts: {id: string, text: string}[] = [];
  // posts = [
  //   {id: "1", text: "test"},
  //   {id: "2", text: "test2"},
  // ]
  postService = inject(PostService)
  async ngOnInit(): Promise<void> {
    this.posts = await this.postService.getAllPosts()
    console.log('Component initialized');
  }

  async createPost(text: string){
    await this.postService.createPost(text);
    this.posts = await this.postService.getAllPosts()
  }
  title="list of posts";
}
