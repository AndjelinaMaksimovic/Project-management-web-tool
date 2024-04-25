import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';
import { MatSnackBar } from '@angular/material/snack-bar';

export type Comment = {
  author: string,
  date: Date,
  content: string,
}
function mapComment(apiComment: any) {
  return {
    author: `${apiComment.firstName} ${apiComment.lastName}`,
    date: new Date(apiComment.commentDate),
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

  constructor(private http: HttpClient, private snackBar: MatSnackBar) {}

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

  public async postComment(comment: string){
    try {
      const res = await firstValueFrom(
        this.http.post<any>(environment.apiUrl + `/Task/createNewTaskComment`, 
        { comment: comment, taskId: this.context.taskId },
        {...this.httpOptions, responseType: 'text' as 'json',}
        )
      );
      this.snackBar.open("comment posted!", undefined, {
        duration: 2000,
      });
    } catch (e) {
      console.log(e);
      this.snackBar.open("there was an error posting your comment", undefined, {
        duration: 2000,
      });
    }
    await this.fetchComments();
  }
}