import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProjectsService {

  constructor(private http: HttpClient) { }

  
  async createNew(
    obj: any
  ): Promise<boolean> {
    try {
      const res = await firstValueFrom(
        this.http.post<any>(
          environment.apiUrl +
            `/createNewProject`,
          obj,
          environment.httpOptions
        )
      );
      if (!res.ok) return false;
      return true;
    } catch (e) {
      console.log(e);
    }
    return false;
  }
}
