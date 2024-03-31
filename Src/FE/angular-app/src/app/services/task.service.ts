import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';
import { StatusService } from './status.service';

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

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  constructor(private http: HttpClient, private statusService: StatusService) {}

  /** in-memory task cache */
  private tasks: Task[] = [];

  /**
   * this function maps task data from the backend to the frontend task format
   * @param apiTask task from the backend response
   * @returns task in the app format
   * @remarks TODO apiTask format is any and this is not safe. Maybe use Zod for response validation
   */
  mapTask(apiTask: any): Task {
    return {
      title: apiTask.name,
      description: apiTask.description,
      // priority: apiTask.priority,
      // status: apiTask.status,
      // category: apiTask.category,
      priority: (['Low', 'Medium', 'High'] as const)[apiTask.taskId % 3],
      // status: (['Active', 'Close', 'Past Due'] as const)[apiTask.taskId % 3],
      status: this.statusService.idToName(apiTask.statusId) || 'unknown',
      category: (['Finance', 'Marketing', 'Development'] as const)[
        apiTask.taskId % 3
      ],
      id: apiTask.taskId,
      date: new Date(Date.parse(apiTask.dueDate)),
    };
  }

  /** for which project/user should we fetch tasks? */
  private context: {
    projectId?: number;
  } = {};

  private httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    withCredentials: true,
    observe: 'response' as 'response',
  };

  /**
   * @returns a list of tasks (current task cache)
   * @note this doesn't fetch task data from the server, it just returns the current task cache
   */
  public getTasks() {
    return this.tasks;
  }

  /**
   * we use context to determine which tasks we work with.
   * for what project/user should the tasks be fetched
   * @param context new context
   */
  public setContext(context: { projectId?: number } = {}) {
    this.context = { ...this.context, ...context };
    // after changing the context, we need to clear the previous tasks cache
    this.tasks = [];
  }
  /**
   * This function is used to update the current tasks cache.
   * It fetches task data from the backend.
   * Use it to initialize tasks list or
   * after deleting/updating/creating tasks
   * @param context optional context value
   */
  public async fetchTasks(context?: { projectId: number }) {
    if (context) this.setContext(context);
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl +
            `/Task/projectTasks?projectId=${this.context.projectId}`,
          this.httpOptions
        )
      );
      await this.statusService.fetchStatuses();
      this.tasks = res.body.map((task: any) => {
        return this.mapTask(task);
      });
    } catch (e) {
      console.log(e);
    }
    return false;
  }

  /**
   * this function takes a partial task object and updates the corresponding task accordingly
   * @param task partial task object. Must have Id
   */
  async updateTask(task: Partial<Task> & Pick<Task, 'id'>) {
    try {
      const request: Record<string, unknown> = { taskId: task.id };
      if (task.status)
        request['statusId'] = this.statusService.nameToId(task.status);
      if (task.title) request['name'] = task.title;
      if (task.description) request['description'] = task.description;
      if (task.date) request['dueDate'] = task.date;
      const res = await firstValueFrom(
        this.http.put<any>(environment.apiUrl + `/Task/updateTask`, request, {
          ...this.httpOptions,
        })
      );
      await this.fetchTasks();
    } catch (e) {
      console.log(e);
    }
  }
  /**
   * this function changes the given task's status to archived and automatically re-fetches the task cache
   * @param taskId id of the task to delete
   */
  async deleteTask(taskId: number) {
    try {
      const res = await firstValueFrom(
        this.http.put<any>(
          environment.apiUrl + `/Task/updateTask`,
          {
            taskId: taskId,
            statusId: 1,
          },
          {
            ...this.httpOptions,
          }
        )
      );
      await this.fetchTasks();
    } catch (e) {
      console.log(e);
    }
  }

  async createTask(task: Omit<Task, 'id'>, projectId: number) {
    try {
      const res = await firstValueFrom(
        this.http.post<any>(
          environment.apiUrl + `/Task/createNewTask`,
          {
            name: task.title,
            description: task.description,
            dueDate: task.date.toISOString(),
            statusId: 1,
            priorityId: 1,
            difficultyLevel: 1,
            categoryId: 1,
            dependencyIds: [],
            projectId: this.context.projectId,
          },
          {
            ...this.httpOptions,
          }
        )
      );
      await this.fetchTasks();
    } catch (e) {
      console.log(e);
    }
  }
}
