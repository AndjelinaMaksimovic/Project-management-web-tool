import { Injectable } from '@angular/core';

export type Comment = {
  author: string,
  date: Date,
  content: string,
}

@Injectable({
  providedIn: 'root'
})
export class CommentsService {
  comments: Comment[] = [
    {
      author: "John Doe",
      date: new Date(),
      content: "This is the first comment"
    },
    {
      author: "Jane Doe",
      date: new Date(),
      content: "This also supports **markdown**\n\n[<img src=\"http://www.google.com.au/images/nav_logo7.png\">](http://google.com.au/)"
    },
  ];
  getComments(){
    return this.comments;
  }
  constructor() { }
}
