import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { product } from 'src/app/models/product';
import { ProductService } from 'src/app/services/product.service';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.css']
})
export class EditComponent implements OnInit {

  @Input () Product? : product;
  @Output () Updated1=new EventEmitter<product[]>();
  constructor(private service:ProductService) { }

  ngOnInit(): void {
  }

  updateProduct(p:product){
    this.service.updateProduct(p).subscribe((products:product[]) =>this.Updated1.emit(products));
  }
  deleteProduct(p:product){
    this.service.deleteProduct(p).subscribe((products:product[]) =>this.Updated1.emit(products));
  }
  createProduct(p:product){
    this.service.createProduct(p).subscribe((products:product[]) =>this.Updated1.emit(products));
  }

}
