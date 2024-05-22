import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';
import { MatSnackBar } from '@angular/material/snack-bar';

export type User = {
  email: string;
  firstName: string;
  lastName: string;
  id: number;
  roleName: string;
  roleId: number;
  profilePicture: string;
  projects: any;
};
function mapUser(apiUser: any) {
  return {
    id: apiUser.id,
    email: apiUser.email,
    firstName: apiUser.firstname,
    lastName: apiUser.lastname,
    roleName: apiUser.roleName,
    roleId: apiUser.roleId,
    profilePicture: apiUser.profilePicture,
    projects: apiUser.projects,
  };
}

@Injectable({
  providedIn: 'root',
})
export class UserService {
  /** in-memory task cache */
  private users: User[] = [];
  private httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    withCredentials: true,
    observe: 'response' as 'response',
  };

  constructor(private http: HttpClient, private snackBar: MatSnackBar) {}
  public getUsers() {
    return this.users;
  }
  public async fetchUsersByRole(roleId: number) {
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl + `/User/getUsers?roleId=${roleId}`,
          this.httpOptions
        )
      );
      this.users = res.body.map((user: any) => {
        return mapUser(user);
      });
    } catch (e) {
      console.log(e);
    }
    return false;
  }

  public async fetchUsers() {
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl + `/User/getUsers`,
          this.httpOptions
        )
      );
      this.users = res.body.map((user: any) => {
        return mapUser(user);
      });
    } catch (e) {
      console.log(e);
    }
    return false;
  }

  public async fetchUsersByProject(projectId: number) {
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl + `/User/getUsers?ProjectId=${projectId}`,
          this.httpOptions
        )
      );
      this.users = res.body.map((user: any) => {
        return mapUser(user);
      });
    } catch (e) {
      console.log(e);
    }
    return false;
  }
  public async getUser(userId: number) {
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl + `/User/getUser?userId=${userId}`,
          this.httpOptions
        )
      );
      return res.body[0];
    } catch (e) {
      console.log(e);
    }
    return false;
  }

  public async getMe() {
    try {
      const res = await firstValueFrom(
        this.http.get<any>(
          environment.apiUrl + `/User/getMyData`,
          this.httpOptions
        )
      );
      return res.body;
    } catch (e) {
      console.log(e);
    }
    return false;
  }
  
  async userRole(userId: number){
    try {
      const res = await firstValueFrom(
        this.http.post<any>(
          environment.apiUrl + `/User/userRole`,
          {
            userId: userId,
          },
          {
            ...this.httpOptions
          }
        )
      );
      return res.body
    } catch (e) {
      console.log(e);
      return false
    }
  }
}
