import { Component, EventEmitter, Output, Input } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-password-field',
  standalone: true,
  imports: [MaterialModule, FormsModule],
  templateUrl: './password-field.component.html',
  styleUrl: './password-field.component.css'
})
export class PasswordFieldComponent {
  @Input() label: string = "";
  @Output() eventFromChild = new EventEmitter<string>();

  hide: boolean = true;
  password: string = "";
  sendEvent() {
     this.eventFromChild.emit(this.password);
  }
}
