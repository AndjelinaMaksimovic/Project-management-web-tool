import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { firstValueFrom } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root',
})
export class RolesService {
  private httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    withCredentials: true,
    observe: 'response' as 'response',
  };

  constructor(private http: HttpClient, private snackBar: MatSnackBar) {}

  public async getAllRoles() {
    try {
      const r = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl + '/Role/allRoles',
          { ...environment.httpOptions }
        )
      );
      const roles: {roleName: string, id: number, permissions: {name: string, value: boolean}[]}[] = r.body.map((role: any) => ({
        id: role.roleId,
        roleName: role.roleName,
        permissions: Object.entries(role).slice(2).map(p => ({name: p[0], value: p[1]})),
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
  
  public async createCustomRole(name: string, permissions: string[]) {
    try {
      const r = await firstValueFrom(
        this.http.post<any>(environment.apiUrl + `/Role/createCustomRole`, 
        {
          customRoleName: name,
          permissions: permissions,
        },
        this.httpOptions
        )
      )
      return r.body.roleId
    } catch (e) {
      console.log(e);
      if(e instanceof HttpErrorResponse){
        this.snackBar.open(e?.error?.errorMessage, undefined, {
          duration: 2000,
        });
      }
      return false
    }
  }

}
