import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';
import { StatusService } from './status.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CategoryService } from './category.service';
import { LocalStorageService } from './localstorage';
import { PriorityService } from './priority.service';
import { UserService } from './user.service';

/**
 * Task format used within the app
 */
export type Task = Readonly<{
  title: string;
  description: string;
  category: string;
  categoryId: number;
  priority: 'Low' | 'Medium' | 'High';
  status: string;
  startDate: Date;
  dueDate: Date;
  id: number;
  index: number;
  indexInCategory: number;
  projectId?: number | undefined;
  assignedTo: any;
  dependentTasks: {taskId: number, typeOfDependencyId: number}[];
  progress: number;
  archived: boolean;
}>;

@Injectable({
  providedIn: 'root',
})
export class TaskService {
    constructor(private http: HttpClient, private statusService: StatusService, private priorityService: PriorityService, private categoryService: CategoryService, private snackBar: MatSnackBar, private localStorageService: LocalStorageService, private userService: UserService) {}

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
      priority: apiTask.priorityName,
      status: apiTask.statusName,
      category: apiTask.categoryName,
      categoryId: apiTask.categoryId,
      id: apiTask.taskId,
      index: apiTask.index,
      indexInCategory: apiTask.indexInCategory,
      projectId: apiTask.projectId,
      startDate: new Date(Date.parse(apiTask.startDate)),
      dueDate: new Date(Date.parse(apiTask.dueDate)),
      assignedTo: apiTask.assignedTo,
      dependentTasks: apiTask.dependentTasks,
      progress: apiTask.progress,
      archived: apiTask.archived,
    };
  }

  /** for which project/user should we fetch tasks? */
  private context: {
    projectId?: number;
    assignedTo?: number;
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
  public setContext(context: { projectId?: number, assignedTo?: number } = {}) {
    this.context = { ...this.context, ...context };
    // after changing the context, we need to clear the previous tasks cache
    this.statusService.setContext(context);
    this.categoryService.setContext(context);
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
            `/Task/projectTasks?projectId=${this.context.projectId}&includeArchived=true`,
          this.httpOptions
        )
      );
      await this.statusService.fetchStatuses();
      await this.priorityService.fetchPriorities();
      await this.categoryService.fetchCategories();
      this.tasks = res.body.map((task: any) => {
        return this.mapTask(task);
      });
    } catch (e) {
      console.log(e);
    }
    return false;
  }

  public async fetchUserTasks(context?: { projectId?: number, assignedTo: number }) {
    if (context) this.setContext(context);
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl +
            (this.context.projectId ?
              `/Task/projectTasks?projectId=${this.context.projectId}&assignedTo=${this.context.assignedTo}` :
              `/Task/projectTasks?assignedTo=${this.context.assignedTo}`
            ),
          this.httpOptions
        )
      );
        await this.statusService.fetchStatuses();
        await this.categoryService.fetchCategories();
      this.tasks = res.body.map((task: any) => {
        return this.mapTask(task);
      });
    } catch (e) {
      console.log(e);
    }
    return false;
  }

  public async fetchTasksFromLocalStorage(projectId: number, filterName: string) {
    this.setContext({projectId});
    let data = this.localStorageService.getData(filterName);
    data = { ...data, projectId: projectId };

    if(!data.assignedTo || data.assignedTo != -1) {
      let userId = (await this.userService.getMe()).id;
      data = { ...data, assignedTo: userId };
    }
    else {
      data.assignedTo = '';
    }
    
    let params = new HttpParams({ fromObject: data });
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl + '/Task/projectTasks',
          { ...environment.httpOptions, params: params }
        )
      );
      await this.statusService.fetchStatuses();
      await this.priorityService.fetchPriorities();
      await this.categoryService.fetchCategories();
      this.tasks = res.body.map((task: any) => {
        return this.mapTask(task);
      });
    } catch (e) {
      console.log(e);
    }
    return false;
  }

  public async fetchTasksFromLocalStorageContext(filterName: string, context?: { projectId: number }) {
    if (context) this.setContext(context);
    let data = this.localStorageService.getData(filterName);
    data = { ...data, projectId: this.context.projectId };

    if(!data.assignedTo || data.assignedTo != -1) {
      let userId = (await this.userService.getMe()).id;
      data = { ...data, assignedTo: userId };
    }
    else {
      data.assignedTo = '';
    }
    
    let params = new HttpParams({ fromObject: data });
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl + '/Task/projectTasks',
          { ...environment.httpOptions, params: params }
        )
      );
      await this.statusService.fetchStatuses();
      await this.priorityService.fetchPriorities();
      await this.categoryService.fetchCategories();
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
  // async updateTask(task: Partial<Task> & Pick<Task, 'id'> & {
  //   categoryId?: string | undefined,
  //   statusId?: string | undefined,
  //   priorityId?: string | undefined,
  //   userId?: string | undefined,
  // }) {
  async updateTask(task: any) {
    try {
      const request: Record<string, unknown> = { taskId: task.id };
      if (task.status)
        request['statusId'] = this.statusService.nameToId(task.status);
      if(task.index) request["index"] = task.index;
      if(task.categoryId) request["categoryId"] = task.categoryId;
      if(task.statusId) request["statusId"] = task.statusId;
      if(task.priorityId) request["priorityId"] = task.priorityId;
      if(task.userIds) request["userIds"] = task.userIds;
      if (task.title) request['name'] = task.title;
      if (task.description) request['description'] = task.description;
      if (task.startDate) request['startDate'] = task.startDate;
      if (task.dueDate) request['dueDate'] = task.dueDate;
      if (task.dependentTasks) request['dependentTasks'] = task.dependentTasks;
      // update cache
      const index = this.tasks.findIndex((t) => t.id === task.id);
      this.tasks[index] = {...this.tasks[index], ...task};
      const res = await firstValueFrom(
        this.http.put<any>(environment.apiUrl + `/Task/updateTask`, request, {
          ...this.httpOptions,
        })
      );
      await this.fetchTasksFromLocalStorageContext("task_filters");
      this.snackBar.open("Task updated successfully", undefined, {
        duration: 2000,
      });
      return true;
    } catch (e) {
      let error = "";
      if(e instanceof HttpErrorResponse) {
        error = " - " + e.error.errorMessage;
      }
      this.snackBar.open("We couldn't update task" + error, undefined, {
        duration: 8000,
      });
      await this.fetchTasksFromLocalStorageContext("task_filters");
      return false;
    }
    return false;
  }
  /**
   * this function changes the given task's status to archived and automatically re-fetches the task cache
   * @param taskId id of the task to delete
   */
  async deleteTask(taskId: number) {
    try {
      const res = await firstValueFrom(
        this.http.put<any>(
          environment.apiUrl + `/Task/archiveTask`,
          {
            taskId: taskId,
          },
          {
            ...this.httpOptions,
          }
        )
      );
    } catch (e) {
      console.log(e);
    }
    await this.fetchTasksFromLocalStorageContext("task_filters");
  }

  async createTask(task: {
    title: string;
    description: string;
    startDate: Date;
    dueDate: Date;
    status: string;
    priority: string;
    category: string;
    dependencies: string[];
    assignedTo: any;
  }, projectId: number) {
    try {
      const res = await firstValueFrom(
        this.http.post<any>(
          environment.apiUrl + `/Task/createNewTask`,
          {
            name: task.title,
            description: task.description,
            startDate: task.startDate.toISOString(),
            dueDate: task.dueDate.toISOString(),
            statusId: task.status,
            priorityId: task.priority,
            difficultyLevel: 1,
            categoryId: task.category,
            dependencyIds: [],
            typeOfDependencyIds: [],
            projectId: this.context.projectId,
            userIds: task.assignedTo,
          },
          {
            ...this.httpOptions,
          }
        )
      );
      await this.fetchTasksFromLocalStorageContext("task_filters");
    } catch (e) {
      console.log(e);
    }
  }

  async createTaskDependency(task: {
    taskId: number;
    dependentTaskId: number;
    typeOfDependencyId: number;
  }) {
    try {
      const res = await firstValueFrom(
        this.http.post<any>(
          environment.apiUrl + `/Task/createTaskDependency`,
          {
            taskId: task.taskId,
            dependentTaskId: task.dependentTaskId,
            typeOfDependencyId: task.typeOfDependencyId,
          },
          {...this.httpOptions, responseType: "text" as "json"}
        )
      );
    } catch (e) {
      let error = "";
      if(e instanceof HttpErrorResponse) {
        error = " - " + JSON.parse(e.error).errorMessage;
      }
      this.snackBar.open("We couldn't create dependency" + error, undefined, {
        duration: 2000,
      });
      return false
    }
    await this.fetchTasksFromLocalStorageContext("task_filters");
    return true
  }

  async deleteDependency(taskId: number, dependentTaskId: number) {
    try {
      const res = await firstValueFrom(
        this.http.delete<any>(
          environment.apiUrl + `/Task/deleteTaskDependency`, {
            ...this.httpOptions,
            responseType: "text" as "json",
            body: { 
              taskId: taskId,
              dependentTaskId: dependentTaskId,
            }
          }
        )
      );
    } catch (e) {
      console.log(e);
      return false
    }
    await this.fetchTasksFromLocalStorageContext("task_filters");
    return true
  }
  async changeTaskProgress(taskId: number, progress: number){
    try {
      await firstValueFrom(
        this.http.post<any>(
          environment.apiUrl + `/Task/changeTaskProgress`,
          {
            taskId: taskId,
            progress: progress,
          },
          {
            ...this.httpOptions
          }
        )
      );
      this.snackBar.open("Progress updated successfully", undefined, {
        duration: 2000,
      });
      return true
    } catch (e) {
      console.log(e);
      this.snackBar.open("Progress updated successfully", undefined, { // TODO: Greska jer api vraca JSON error
      // this.snackBar.open("Failed to update progerss", undefined, {
        duration: 2000,
      });
      return false
    }
  }
  public async getOverdueTasks(projectId: number) {
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl +
            `/Task/projectTasks?projectId=${projectId}`,
          this.httpOptions
        )
      );
      let tasks = res.body.map((task: any) => {
        return this.mapTask(task);
      });
      return tasks.filter((task: Task) => new Date(task.dueDate) < new Date()).length;
    } catch (e) {
      console.log(e);
    }
    return 0;
  }
}
