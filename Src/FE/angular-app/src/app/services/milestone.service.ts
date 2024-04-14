import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MilestoneService {

  private milestones: any[] = []

  constructor(private http: HttpClient) { }

  mapMilestones(apiMilestone: any){
    return {
      
    }
  }

  private context: {
    projectId?: number;
  } = {};
  public setContext(context: { projectId?: number } = {}) {
    this.context = { ...this.context, ...context };
    this.milestones = [];
  }
  
  public async fetchMilestones(context?: { projectId: number }) {
    if (context) this.setContext(context);
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl +
            `/Task/projectTasks?projectId=${this.context.projectId}`,
          environment.httpOptions
        )
      );
      this.milestones = res.body.map((milestone: any) => {
        return this.mapMilestones(milestone);
      });
    } catch (e) {
      console.log(e);
    }
    return false;
  }
}
