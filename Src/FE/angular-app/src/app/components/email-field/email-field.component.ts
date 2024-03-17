import { Component, EventEmitter, Output, Input } from '@angular/core';
import { ClearableInputComponent } from '../clearable-input/clearable-input.component';

@Component({
  selector: 'app-email-field',
  standalone: true,
  imports: [ClearableInputComponent],
  templateUrl: './email-field.component.html',
  styleUrl: './email-field.component.css'
})
export class EmailFieldComponent {
  @Input() label: string = "Email";
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
