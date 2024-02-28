import { Component } from '@angular/core';
import { PostsComponent } from '../../components/posts/posts.component';

@Component({
  selector: 'app-testroute',
  standalone: true,
  imports: [PostsComponent],
  templateUrl: './testroute.component.html',
  styleUrl: './testroute.component.css'
})
export class TestrouteComponent {

}
