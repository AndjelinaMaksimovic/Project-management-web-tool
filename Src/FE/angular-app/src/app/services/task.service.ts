import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';

type task = Readonly<{
  title: string;
  category: string;
  priority: 'Low' | 'Medium' | 'High';
  status: string;
  date: Date;
  id: number;
}>;
/**
 * this function maps task data from the backend to the frontend task format
 * @param apiTask task from the backend response
 * @returns task in the app format
 * @remarks TODO apiTask format is any and this is not safe. Maybe use Zod for response validation
 */
function mapTask(apiTask: any): task {
  return {
    title: apiTask.name,
    priority: apiTask.priority,
    status: apiTask.status,
    category: apiTask.category,
    id: apiTask.id,
    date: new Date(Date.parse(apiTask.dueDate)),
  };
}

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  private tasks: task[] = [];
  private httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    withCredentials: true,
    observe: 'response' as 'response',
  };

  constructor(private http: HttpClient) {}

  public async fetchTasks() {
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl + '/Task/projectTasks?projectId=1',
          this.httpOptions
        )
      );
      this.tasks = res.body.map((task: any) => {
        return mapTask(task);
      });
    } catch (e) {
      console.log(e);
    }
    return false;
  }

  getTasks() {
    return this.tasks;
  }

  async deleteTask(taskId: number) {
    try {
      const res = await firstValueFrom(
        this.http.delete<any>(environment.apiUrl + `/Task/tasksDeletion`, {
          ...this.httpOptions,
          body: { taskId: taskId },
        })
      );
      await this.fetchTasks();
    } catch (e) {
      console.log(e);
    }
  }
}
