import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AvatarService {

  getProfileImagePath(userId: number | string){
    return `${environment.apiUrl}/User/users/avatars/${userId}?timestamp=${Math.round(Date.now() / (1000 * 10))}`;
  }
}
