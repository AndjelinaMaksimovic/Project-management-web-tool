import { Injectable } from '@angular/core';

import {product} from '../models/product';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private url="Values";

  constructor(private http: HttpClient) { }

  public getProduct() : Observable<product[]> {
    return this.http.get<product[]>(`${environment.apiUrl}/${this.url}`);
    
  }

  public updateProduct(Product: product) : Observable<product[]> {
    return this.http.put<product[]>(`${environment.apiUrl}/${this.url}`,Product);
    
  }

  public deleteProduct(Product: product) : Observable<product[]> {
    return this.http.delete<product[]>(`${environment.apiUrl}/${this.url}/${Product.id}`);
    
  }

  public createProduct(Product: product) : Observable<product[]> {
    return this.http.post<product[]>(`${environment.apiUrl}/${this.url}`,Product);
    
  }
}

