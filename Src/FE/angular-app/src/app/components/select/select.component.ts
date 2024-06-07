// import { Component } from '@angular/core';

// @Component({
//   selector: 'app-select',
//   standalone: true,
//   imports: [],
//   templateUrl: './select.component.html',
//   styleUrl: './select.component.css'
// })
// export class SelectComponent {

// }
import { Component, EventEmitter, Output, Input } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-select',
  standalone: true,
  imports: [MaterialModule, FormsModule],
  templateUrl: './select.component.html',
  styleUrl: './select.component.css',
})
export class SelectComponent {
  @Input() options: {value: any, viewValue: string}[] = [];
  @Input() label: string = "";
  @Input() placeholder: string = "";
  @Input() multiple: boolean = false;
  @Output() valueChange = new EventEmitter<any>();
  @Input() value: any;
  @Input() required: boolean = false;
  get data(){
    return this.value;
  }
  set data(value: any){
    this.value = value;
    this.valueChange.emit(this.data);
  }
}
