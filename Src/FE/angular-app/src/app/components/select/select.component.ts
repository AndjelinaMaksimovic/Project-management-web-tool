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
  @Input() multiple: boolean = false;
  @Output() valueChange = new EventEmitter<string>();
  @Input() value: string = "";
  get data(){
    return this.value;
  }
  set data(value: string){
    this.value = value;
    this.valueChange.emit(this.data);
  }
}
