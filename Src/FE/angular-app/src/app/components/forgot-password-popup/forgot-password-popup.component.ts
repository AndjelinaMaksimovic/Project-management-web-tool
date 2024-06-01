import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog'
import { MaterialModule } from '../../material/material.module';
import { FormsModule } from '@angular/forms';
import { EmailFieldComponent } from '../email-field/email-field.component';
import { UserService } from '../../services/user.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-forgot-password-popup',
  standalone: true,
  imports: [ MaterialModule, FormsModule, MatDialogModule, EmailFieldComponent ],
  templateUrl: './forgot-password-popup.component.html',
  styleUrl: './forgot-password-popup.component.css'
})
export class ForgotPasswordPopupComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public data: string, private userService: UserService, private snackBar: MatSnackBar) {}

  async send(){
    const res = await this.userService.sendUpdatePasswordMail(this.data)
    if(res)
      this.snackBar.open("Email sent!", undefined, {
        duration: 2000,
      });
    else
      this.snackBar.open("Invalid email", undefined, {
        duration: 2000,
      });
  }
}
