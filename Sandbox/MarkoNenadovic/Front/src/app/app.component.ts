import { Component } from '@angular/core';
import { product } from './models/product';
import { ProductService } from './services/product.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'MarkoNenadovic.UI';
  products: product[]=[];
  toEdit?:product;


  constructor(private ProductService: ProductService){}

  ngOnInit() : void{
    this.ProductService.getProduct().subscribe((result: product[]) => (this.products=result));
    
  }

  updateList(products:product[]){
    this.products=products;
  }

  initNew(){
    this.toEdit=new product();

  }
  editProduct(Product:product){
    this.toEdit=Product;

  }
}
