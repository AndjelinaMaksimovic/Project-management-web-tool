import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PostService } from '../../services/post.service';
import { UpdatePostComponent } from '../../components/update-post/update-post.component';

@Component({
  selector: 'app-details',
  standalone: true,
  imports: [UpdatePostComponent],
  templateUrl: './details.component.html',
  styleUrl: './details.component.css'
})
export class DetailsComponent {
  route: ActivatedRoute = inject(ActivatedRoute);
  postService = inject(PostService);
  postId: string = "";
  postData: undefined | {id: string, text: string} = undefined;
  constructor(private router: Router){
    this.postId = this.route.snapshot.params["id"];
  }
  async ngOnInit(){
    const postData = await this.postService.getPost(this.postId);
    this.postData = postData;
    console.log(postData);
  }

  async deletePost() {
    const postId = this.postId;
    await this.postService.deletePost(postId);
    this.router.navigate(["posts"]);
  }

  async updatePost(text: string){
    await this.postService.updatePost(this.postId, text);
    const postData = await this.postService.getPost(this.postId);
    this.postData = postData;
  }
}
