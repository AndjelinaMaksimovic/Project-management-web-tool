import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';
import { LocalStorageService } from './localstorage';

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
    starred: apiProject.starred,
  };
}

@Injectable({
  providedIn: 'root',
})
export class ProjectService {
  /** in-memory project cache */
  private projects: Project[] = [];

  constructor(private http: HttpClient, private localStorageService : LocalStorageService) {}

  /**
   * @returns a list of projects (current project cache)
   * @note this doesn't fetch project data from the server, it just returns the current project cache
   */
  public getProjects() {
    return this.projects;
  }

  public getProjectWithID(projectId: number) {
    return this.projects.find(project => project.id == projectId);
  }
  
  /**
   * This function is used to update the current project cache.
   * It fetches project data from the backend.
   * Use it to initialize projects list or
   * after deleting/updating/creating projects
   */
  public async fetchProjects() {
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl + '/Projects/filterProjects',
          environment.httpOptions,
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
}
