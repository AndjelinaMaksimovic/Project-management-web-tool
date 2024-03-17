import { Component } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ClearableInputComponent } from '../../components/clearable-input/clearable-input.component';
import { EmailFieldComponent } from '../../components/email-field/email-field.component';
import { SelectComponent } from '../../components/select/select.component';
import { MatDialog } from '@angular/material/dialog';
import { InviteModalComponent } from '../../components/invite-modal/invite-modal.component';
@Component({
  selector: 'app-register',
  standalone: true,
  imports: [MaterialModule, FormsModule, CommonModule, ClearableInputComponent, EmailFieldComponent, MaterialModule, SelectComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  constructor(
    private dialogue: MatDialog,
  ) {}
  ngOnInit() {
    this.popup();
  }
  popup(){
    this.dialogue.open(InviteModalComponent, { autoFocus: false })
  }
}
