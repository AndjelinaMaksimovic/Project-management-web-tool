import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';

export type Comment = {
  author: string,
  date: Date,
  content: string,
}
function mapComment(apiComment: any) {
  return {
    author: "John Doe",
    date: new Date(),
    content: apiComment.comment,
    id: apiComment.commentId,
  };
}

@Injectable({
  providedIn: 'root',
})
export class CommentsService {
  private comments: Comment[] = [];
  private context: {
    taskId?: number;
  } = {};

  private httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    withCredentials: true,
    observe: 'response' as 'response',
  };

  constructor(private http: HttpClient) {}

  public setContext(context: { taskId?: number } = {}) {
    this.context = { ...this.context, ...context };
    this.comments = [];
  }

  public getComments() {
    return this.comments;
  }

  public async fetchComments(context?: { taskId?: number }) {
    if(context) this.setContext(context);
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl + `/Task/TaskComments?TaskId=${this.context.taskId}`,
          this.httpOptions
        )
      );
      this.comments = res.body.map((comm: any) => {
        return mapComment(comm);
      });
    } catch (e) {
      console.log(e);
    }
    return false;
  }
}