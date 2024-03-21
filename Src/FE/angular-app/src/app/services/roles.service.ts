import { HttpClient, HttpHeaders } from '@angular/common/http';
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
      const roles: {roleName: string, id: number}[] = r.roleNames.map((roleName: string, index: number) => ({
        roleName: roleName,
        id: r.rolesIds[index],
      }));
      return roles;
    } catch (e) {
      console.log(e);
    }
    return null;
  }
}
