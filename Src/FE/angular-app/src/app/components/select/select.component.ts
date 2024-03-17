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
  @Input() options: {value: string, viewValue: string}[] = [];
  @Input() label: string = "";
  @Input() placeholder: string = "";
  @Output() valueChange = new EventEmitter<string>();

  private _data: string = "";
  get data(){
    return this._data;
  }
  set data(value: string){
    this._data = value;
    this.valueChange.emit(this.data);
  }
}
