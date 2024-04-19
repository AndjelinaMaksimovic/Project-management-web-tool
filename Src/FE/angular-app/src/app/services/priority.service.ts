import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';

export type Priority = Readonly<{
  id: number;
  name: string;
}>;

function mapPriority(apiPriority: any): Priority {
  return {
    name: apiPriority.name,
    id: apiPriority.id,
  };
}

@Injectable({
  providedIn: 'root',
})

export class PriorityService {
  /** in-memory project cache */
  private priorities: Priority[] = [];

  constructor(private http: HttpClient) {}

  /**
   * @returns a list of projects (current project cache)
   * @note this doesn't fetch project data from the server, it just returns the current project cache
   */
  public getPriorities() {
    return this.priorities;
  }

  public getPrioritiesWWithID(priorityId: number) {
    return this.priorities.find(priority => priority.id == priorityId);
  }
  
  /**
   * This function is used to update the current project cache.
   * It fetches project data from the backend.
   * Use it to initialize projects list or
   * after deleting/updating/creating projects
   */
  public async fetchPriorities() {
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl + '/Priority/allPriorities',
          environment.httpOptions,
        )
      );
      this.priorities = res.body.map((project: any) => {
        return mapPriority(project);
      });
    } catch (e) {
      console.log(e);
    }
    return false;
  }
}
