import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class RolesService {
  private httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    withCredentials: true,
    observe: 'response' as 'response',
  };

  constructor(private http: HttpClient) {}

  public async getAllRoles() {
    try {
      const r = await firstValueFrom(
        this.http.get<any>(environment.apiUrl + '/Role/allRoles', {})
      );
      const roles: {roleName: string, id: number}[] = r.map((r: any) => ({
        roleName: r.roleName,
        id: r.roleId,
      }));
      console.log(roles)
      return roles;
    } catch (e) {
      console.log(e);
    }
    return null;
  }

  public async getProjectRoles(projectId: number) {
    try {
      let params = new HttpParams({ fromObject: { projectId: projectId} });
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl + `/Role/getProjectRoles`,
          { ...environment.httpOptions, params: params }
        )
      );
      const roles: {roleName: string, id: number}[] = res.body.roleNames.map((roleName: string, index: number) => ({
        roleName: roleName,
        id: res.body.rolesIds[index],
      }));
      return roles;
    } catch (e) {
      console.log(e);
    }
    return null;
  }
}
