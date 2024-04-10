import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';
import { MatSnackBar } from '@angular/material/snack-bar';

type User = {
  email: string;
  firstName: string;
  lastName: string;
  id: number;
  roleName: string;
  roleId: number;
  profilePicture: string;
};
function mapUser(apiUser: any) {
  return {
    email: apiUser.name,
    firstName: apiUser.firstname,
    lastName: apiUser.lastname,
    roleName: apiUser.roleName,
    roleId: apiUser.roleId,
    profilePicture: apiUser.profilePicture,
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
  /**
   * @returns a list of tasks (current task cache)
   * @note this doesn't fetch task data from the server, it just returns the current task cache
   */
  public getUsers() {
    return this.users;
  }
  /**
   * This function is used to update the current tasks cache.
   * It fetches task data from the backend.
   * Use it to initialize tasks list or
   * after deleting/updating/creating tasks
   * @param context optional context value
   */
  public async fetchUsers() {
    try {
      const res = await firstValueFrom(
        this.http.post<any>(
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
}
