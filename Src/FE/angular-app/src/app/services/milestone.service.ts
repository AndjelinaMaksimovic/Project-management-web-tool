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
      title: apiMilestone.name,
      description: apiMilestone.description,
      // priority: (['Low', 'Medium', 'High'] as const)[apiMilestone.taskId % 3],
      // status: this.statusService.idToName(apiMilestone.statusId) || 'unknown',
      category: (['Finance', 'Marketing', 'Development'] as const)[
        apiMilestone.taskId % 3
      ],
      id: apiMilestone.taskId,
      projectId: this.context.projectId,
      startDate: new Date(Date.parse(apiMilestone.startDate)),
      dueDate: new Date(Date.parse(apiMilestone.dueDate)),
      assignedTo: apiMilestone.assignedTo,
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
            `/Milestone/projectMielstones?projectId=${this.context.projectId}`,
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
  public getMilestones() {
    return this.milestones;
  }
}
