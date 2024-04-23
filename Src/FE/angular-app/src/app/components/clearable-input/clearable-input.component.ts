import { Component, EventEmitter, Output, Input } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-clearable-input',
  standalone: true,
  imports: [MaterialModule, FormsModule],
  templateUrl: './clearable-input.component.html',
  styleUrl: './clearable-input.component.css'
})
export class ClearableInputComponent {
  @Input() type: string = "";
  @Input() label: string = "";
  @Input() placeholder: string = "";
  @Input() required: boolean = false;
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
