import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';
import { MatSnackBar } from '@angular/material/snack-bar';

type Status = {
  name: string;
  id: number;
  projectId: number;
  order: number;
};
function mapStatus(apiStatus: any) {
  return {
    name: apiStatus.name,
    id: apiStatus.id,
    projectId: apiStatus.projectId,
    order: apiStatus.order,
  };
}

@Injectable({
  providedIn: 'root',
})
export class StatusService {
  /** in-memory task cache */
  private statuses: Status[] = [];
  private context: {
    projectId?: number;
  } = {};
  private statusIdMap: Partial<Record<number | string, string>> = {};
  private IdStatusMap: Partial<Record<string, number>> = {};

  private httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    withCredentials: true,
    observe: 'response' as 'response',
  };

  constructor(private http: HttpClient, private snackBar: MatSnackBar) {}
  /**
   * we use context to determine what project are we working in.
   * for what project/user should statuses be fetched
   * @param context new context
   */
  public setContext(context: { projectId?: number } = {}) {
    this.context = { ...this.context, ...context };
    // after changing the context, we need to clear the previous status cache
    this.statuses = [];
  }
  /**
   * @returns a list of tasks (current task cache)
   * @note this doesn't fetch task data from the server, it just returns the current task cache
   */
  public getStatuses() {
    return this.statuses;
  }
  public idToName(statusId: number) {
    return this.statusIdMap[statusId];
  }
  public nameToId(name: string) {
    return this.IdStatusMap[name];
  }
  /**
   * This function is used to update the current tasks cache.
   * It fetches task data from the backend.
   * Use it to initialize tasks list or
   * after deleting/updating/creating tasks
   * @param context optional context value
   */
  public async fetchStatuses() {
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl + `/Status/getStatus?projectId=${this.context.projectId}`,
          this.httpOptions
        )
      );
      this.statuses = res.body.map((task: any) => {
        return mapStatus(task);
      });
      this.statusIdMap = Object.fromEntries(
        this.statuses.map((status) => {
          return [status.id, status.name];
        })
      );
      this.IdStatusMap = Object.fromEntries(
        this.statuses.map((status) => {
          return [status.name, status.id];
        })
      );
    } catch (e) {
      console.log(e);
    }
    return false;
  }

  public async deleteStatus(status: number): Promise<void>
  public async deleteStatus(status: string): Promise<void>
  public async deleteStatus(status: number | string) {
    const statusId = typeof status === "string" ? this.nameToId(status) : status;
    console.log(statusId);
    if(statusId === undefined) return;
    try {
      const res = await firstValueFrom(
        this.http.delete<any>(environment.apiUrl + `/Status/deleteStatus`, {
          ...this.httpOptions,
          body: { 
            statusId: statusId,
            projectId: this.context.projectId
          },
        })
      );
    } catch (e) {
      console.log(e);
      if(e instanceof HttpErrorResponse){
        if(e?.error?.errorMessage?.includes("exist on project with ID")){
          this.snackBar.open("We can't delete a status that has active tasks", undefined, {
            duration: 2000,
          });
        } else {
          this.snackBar.open("We couldn't delete this status", undefined, {
            duration: 2000,
          });
        }
      }
    }
    await this.fetchStatuses();
  }
  public async createStatus(name: string) {
    try {
      const res = await firstValueFrom(
        this.http.post<any>(environment.apiUrl + `/Status/createStatus`, 
        { name, projectId: this.context.projectId },
        this.httpOptions
        )
      );
    } catch (e) {
      console.log(e);
    }
    await this.fetchStatuses();
  }

  public async reorderStatuses(newOrder: string[]){
    const idsOrder = newOrder.map((status) => this.nameToId(status));
    try {
      const res = await firstValueFrom(
        this.http.post<any>(environment.apiUrl + `/Status/changeStatusesOrder`, 
        { projectId: this.context.projectId, newOrder: idsOrder },
        this.httpOptions
        )
      );
    } catch (e) {
      console.log(e);
    }
    await this.fetchStatuses();
  }
}
