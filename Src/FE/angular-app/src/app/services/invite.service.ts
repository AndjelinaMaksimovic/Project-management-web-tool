import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root',
})
export class InviteService {
  /** in-memory task cache */
  private httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    withCredentials: true,
    observe: 'response' as 'response',
  };

  constructor(private http: HttpClient, private snackBar: MatSnackBar) {}
 
  public async inviteUser(firstname: string, lastname: string, email: string, roleName: string, roleId: number){
    try {
      const res = await firstValueFrom(
        this.http.post<any>(environment.apiUrl + `/Invites/SendInvite`, 
        {
          firstname: firstname,
          lastname: lastname,
          email: email,
          roleName: roleName,
          roleId: roleId,
        },
        {...this.httpOptions, responseType: "text" as "json"}
        )
      );
      this.snackBar.open("User invited successfully", undefined, {
        duration: 2000,
      });
    } catch (e) {
      this.snackBar.open("We couldn't invite user", undefined, {
        duration: 2000,
      });
      console.log(e);
    }
  }
}
