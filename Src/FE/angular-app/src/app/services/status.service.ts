import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';

type Status = {
  name: string, id: number,
}
function mapStatus(apiStatus: any){
  return {
    name: apiStatus.name,
    id: apiStatus.id,
  }
}


@Injectable({
  providedIn: 'root',
})
export class StatusService {
  /** in-memory task cache */
  private statuses: Status[] = [];
  private statusIdMap: Partial<Record<number | string, string>> = {};
  private IdStatusMap: Partial<Record<string, number>> = {};

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
  public getStatuses() {
    return this.statuses;
  }
  public getStatusMap() {
    return this.statusIdMap;
  }
  public getIdMap() {
    return this.IdStatusMap;
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
          environment.apiUrl +
            `/Status/allStatuses`,
          this.httpOptions
        )
      );
      this.statuses = res.body.map((task: any) => {
        return mapStatus(task);
      });
      this.statusIdMap = Object.fromEntries(this.statuses.map((status) => {
        return [status.id, status.name]
      }));
      this.IdStatusMap = Object.fromEntries(this.statuses.map((status) => {
        return [status.name, status.id]
      }));
    } catch (e) {
      console.log(e);
    }
    return false;
  }
}
