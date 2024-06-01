import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';
import { MatSnackBar } from '@angular/material/snack-bar';

type Category = {
  name: string;
  id: number;
  index: number;
};
function mapCategory(apiCategory: any) {
  return {
    name: apiCategory.name,
    id: apiCategory.id,
    index: apiCategory.index | 0, // 0 for testing
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

  constructor(private http: HttpClient, private snackBar: MatSnackBar) {}

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

  public async createCategory(name: string) {
    try {
      const res = await firstValueFrom(
        this.http.post<any>(environment.apiUrl + `/Category/createNewCategory`, 
        { categoryName: name, projectId: this.context.projectId },
        this.httpOptions
        )
      );
    } catch (e) {
      console.log(e);
    }
    await this.fetchCategories();
  }

  public async deleteCategory(categoryId: number): Promise<void> {
    console.log(categoryId);
    if(categoryId === undefined) return;
    try {
      const res = await firstValueFrom(
        this.http.delete<any>(environment.apiUrl + `/Category/deleteCategory`, {
          ...this.httpOptions,
          body: { 
            categoryId: categoryId
          },
        })
      );
    } catch (e) {
      console.log(e);
      if(e instanceof HttpErrorResponse){
        if(e?.error?.errorMessage?.includes("is already assigned to a task and cannot be deleted")){
          this.snackBar.open("We can't delete a category that has active tasks", undefined, {
            duration: 2000,
          });
        }
        // else {
        //   this.snackBar.open("We couldn't delete this category", undefined, {
        //     duration: 2000,
        //   });
        // }
      }
    }
    await this.fetchCategories();
  }
}
