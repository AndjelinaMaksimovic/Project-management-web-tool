import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';

type Category = {
  name: string;
  id: number;
};
function mapCategory(apiCategory: any) {
  return {
    name: apiCategory.name,
    id: apiCategory.id,
  };
}

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  private categories: Category[] = [];
  private context: {
    projectId?: number;
  } = {};

  private httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    withCredentials: true,
    observe: 'response' as 'response',
  };

  constructor(private http: HttpClient) {}

  public setContext(context: { projectId?: number } = {}) {
    this.context = { ...this.context, ...context };
    this.categories = [];
  }

  public getCategories() {
    return this.categories;
  }

  public async fetchCategories() {
    try {
      const res = await firstValueFrom(
        this.http.post<any>(
          environment.apiUrl + `/Category/allProjectCategories`,
          { projectId: this.context.projectId },
          this.httpOptions
        )
      );
      this.categories = res.body.map((cat: any) => {
        return mapCategory(cat);
      });
    } catch (e) {
      console.log(e);
    }
    return false;
  }
}
