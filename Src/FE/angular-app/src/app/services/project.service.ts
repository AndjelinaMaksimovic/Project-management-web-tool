import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';
import { LocalStorageService } from './localstorage';
import { MatSnackBar } from '@angular/material/snack-bar';

export type Project = Readonly<{
  title: string;
  description: string;
  dueDate: Date;
  startDate: Date;
  starred: boolean;
  archived: boolean;
  id: number;
}>;

function mapProject(apiProject: any): Project {
  return {
    title: apiProject.name,
    description: apiProject.description,
    id: apiProject.id,
    dueDate: new Date(Date.parse(apiProject.dueDate)),
    startDate: new Date(Date.parse(apiProject.startDate)),
    archived: apiProject.archived,
    starred: apiProject.isStarred,
  };
}

@Injectable({
  providedIn: 'root',
})
export class ProjectService {
  /** in-memory project cache */
  private projects: Project[] = [];
  private projectsProgress: Map<number, number> = new Map<number, number>();

  constructor(private http: HttpClient, private localStorageService : LocalStorageService, private snackBar: MatSnackBar) {}

  /**
   * @returns a list of projects (current project cache)
   * @note this doesn't fetch project data from the server, it just returns the current project cache
   */
  public getProjects() {
    return this.projects;
  }

  public getStarredProjects() {
    return this.projects.filter(project => project.starred);
  }

  public getProjectWithID(projectId: number) {
    return this.projects.find(project => project.id == projectId);
  }

  public getProgresses() {
    return this.projectsProgress;
  }

  public getProgress(projectId: number) {
    return this.projectsProgress.get(projectId);
  }

  public updateProgresses() {
    this.projects.forEach(async (project) => {
      this.projectsProgress.set(project.id, await this.getProjectProgress(project.id));
    });
  }
  
  /**
   * This function is used to update the current project cache.
   * It fetches project data from the backend.
   * Use it to initialize projects list or
   * after deleting/updating/creating projects
   */
  public async fetchProjects(filters?: any) {
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl + '/Projects/filterProjects',
          {
            ...environment.httpOptions,
            params: filters,
          },
        )
      );
      this.projects = res.body.map((project: any) => {
        return mapProject(project);
      });
      this.updateProgresses();
    } catch (e) {
      console.log(e);
    }
    return false;
  }

  public async fetchUserProjects(id : number) {
    let params = new HttpParams({ fromObject: { AssignedTo: [ id ]} });
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl + '/Projects/filterProjects',
          { ...environment.httpOptions, params: params }
        )
      );
      this.projects = res.body.map((project: any) => {
        return mapProject(project);
      });
    } catch (e) {
      console.log(e);
    }
    return false;
  }

  public async fetchProjectsLocalStorage(filterName : string) {
    let params = new HttpParams({ fromObject: this.localStorageService.getData(filterName) });
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl + '/Projects/filterProjects',
          { ...environment.httpOptions, params: params }
        )
      );
      this.projects = res.body.map((project: any) => {
        return mapProject(project);
      });
      this.updateProgresses();
    } catch (e) {
      console.log(e);
    }
    return false;
  }

  public async getProjectProgress(projectId : number) {
    let params = new HttpParams({ fromObject: { ProjectId: projectId } });
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl + '/Projects/getProjectProgress',
          { ...environment.httpOptions, params: params }
        )
      );
      return res.body.progressPercentage;
    } catch (e) {
      console.log(e);
    }
    return false;
  }


  async createNew(
    obj: any
  ): Promise<boolean> {
    try {
      const res = await firstValueFrom(
        this.http.post<any>(
          environment.apiUrl +
            `/Projects/createNewProject`,
          obj,
          environment.httpOptions
        )
      );
      await this.fetchProjects();

      if (!res.ok) return false;
      
      return true;
    } catch (e) {
      console.log(e);
    }
    return false;
  }

  async deleteProject(id: number){
    try {
      const res = await firstValueFrom(
        this.http.delete<any>(
          environment.apiUrl +
            `/Projects/projectDeletion`,
          {
            ...environment.httpOptions, body: {id: id}
          }
        )
      );
      return true;
    } catch (e) {
      console.log(e);
    }
    return false;
  }

  async archiveProject(id: number){
    try {
      const res = await firstValueFrom(
        this.http.put<any>(
          environment.apiUrl +
            `/Projects/archiveProject`,
          {
            projectId: id
          },
          {
            ...environment.httpOptions,
            responseType: "text" as "json"
          }
        )
      );
      await this.fetchProjects();
      return true;
    } catch (e) {
      console.log(e);
    }
    return false;
  }

  async unarchiveProject(id: number){
    try {
      const res = await firstValueFrom(
        this.http.delete<any>(
          environment.apiUrl +
            `/Projects/unarchiveProject`,
          {
            ...environment.httpOptions, body: {projectId: id}
          }
        )
      );
      await this.fetchProjects();
      return true;
    } catch (e) {
      console.log(e);
    }
    return false;
  }

  async toggleStarred(id: number) {
    try {
      const res = await firstValueFrom(
        this.http.post<any>(
          environment.apiUrl +
            `/Projects/toggleStarredProject`,
            {
              projectId: id,
            },
            {
              ...environment.httpOptions,
              responseType: "text" as "json"
            }
        )
      );
      await this.fetchProjects();
      return true;
    } catch (e) {
      console.log(e);
    }
    return false;
  }

  async allProjectActivities(projectId: number): Promise<any[]> {
    try {
      const res = await firstValueFrom(
        this.http.post<any>(
          environment.apiUrl +
            `/Projects/allProjectActivities`,
            {
              projectId: projectId,
            },
            {
              ...environment.httpOptions,
            }
        )
      );
      return res.body;
    } catch (e) {
      console.log(e);
    }
    return [];
  }
  
  async allUserActivities(): Promise<any[]> {
    try {
      const res = await firstValueFrom(
        this.http.post<any>(
          environment.apiUrl +
            `/Projects/allUserActivities`,
            {},
            {
              ...environment.httpOptions,
            }
        )
      );
      return res.body;
    } catch (e) {
      console.log(e);
    }
    return [];
  }
  
  async allUsersProjectActivities(): Promise<any[]> {
    try {
      const res = await firstValueFrom(
        this.http.post<any>(
          environment.apiUrl +
            `/Projects/allUsersProjectActivities`,
            {},
            {
              ...environment.httpOptions,
            }
        )
      );
      return res.body;
    } catch (e) {
      console.log(e);
    }
    return [];
  }


  async updateProject(project: any) {
    try {
      const request: Record<string, unknown> = { projectId: project.id };
      if (project.title) request['name'] = project.title;
      if (project.description) request['description'] = project.description;
      if (project.startDate) request['startDate'] = project.startDate;
      if (project.dueDate) request['dueDate'] = project.dueDate;
      // update cache
      const res = await firstValueFrom(
        this.http.put<any>(environment.apiUrl + `/Projects/updateProject`, request, {
          ...environment.httpOptions, responseType: "text" as "json"
        })
      );
      await this.fetchProjectsLocalStorage("project_filters");
      this.snackBar.open("Project updated successfully", undefined, {
        duration: 2000,
      });
      return true;
    } catch (e) {
      console.log(e);
      let error = "";
      if(e instanceof HttpErrorResponse) {
        error = " - " + JSON.parse(e.error).errorMessage;
      }
      this.snackBar.open("We couldn't update project" + error, undefined, {
        duration: 8000,
      });
      await this.fetchProjectsLocalStorage("project_filters");
      return false;
    }
    return false;
  }
}
