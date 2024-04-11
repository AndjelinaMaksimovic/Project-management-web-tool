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
      author: "user1",
      date: new Date(),
      content: "This is the first comment"
    },
    {
      author: "user1",
      date: new Date(),
      content: "test"
    },
  ];
  getComments(){
    return this.comments;
  }
  constructor() { }
}
