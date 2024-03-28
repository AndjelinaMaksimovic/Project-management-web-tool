import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';

/**
 * Task format used within the app
 */
export type Task = Readonly<{
  title: string;
  description: string;
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
function mapTask(apiTask: any): Task {
  return {
    title: apiTask.name,
    description: apiTask.description,
    // priority: apiTask.priority,
    // status: apiTask.status,
    // category: apiTask.category,
    priority: (["Low", "Medium", "High"] as const)[apiTask.taskId % 3],
    status: (["Active", "Close", "Past Due"] as const)[apiTask.taskId % 3],
    category: (["Finance", "Marketing", "Development"] as const)[apiTask.taskId % 3],
    id: apiTask.taskId,
    date: new Date(Date.parse(apiTask.dueDate)),
  };
}

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  /** in-memory task cache */
  private tasks: Task[] = [];
  private httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    withCredentials: true,
    observe: 'response' as 'response',
  };

  constructor(private http: HttpClient) {}

  /**
   * @returns a list of tasks (current task cache)
   * @note this doesn't fetch task data from the server, it just returns the current task cache
   */
  public getTasks() {
    return this.tasks;
  }

  /**
   * This function is used to update the current tasks cache.
   * It fetches task data from the backend.
   * Use it to initialize tasks list or
   * after deleting/updating/creating tasks
   */
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

  /**
   * this function deletes the given task and automatically re-fetches the task cache
   * @param taskId id of the task to delete
   */
  async deleteTask(taskId: number) {
    try {
      const res = await firstValueFrom(
        this.http.delete<any>(environment.apiUrl + `/Task/tasksDeletion`, {
          ...this.httpOptions,
          body: { taskId: taskId },
        })
      );
    } catch (e) {
      console.log(e);
    }
    // TODO this should go inside try block. Delete endpoint parsing error is causing the problem
    await this.fetchTasks();
  }

  async createTask(task: Omit<Task, "id">, projectId: number) {
    try {
      const res = await firstValueFrom(
        this.http.post<any>(environment.apiUrl + `/Task/createNewTask`, 
        {
          "name": task.title,
          "description": task.description,
          "dueDate": task.date.toISOString(),
          "statusId": 1,
          "priorityId": 1,
          "difficultyLevel": 1,
          "categoryId": 1,
          "dependencyIds": [],
          "projectId": projectId
        },
        {
          ...this.httpOptions,
        })
      );
      await this.fetchTasks();
    } catch (e) {
      console.log(e);
    }
  }
}
