import { Component } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCheckboxModule } from '@angular/material/checkbox'
import { MatDialog } from '@angular/material/dialog'
import { ForgotPasswordPopupComponent } from '../../components/forgot-password-popup/forgot-password-popup.component';
import { PasswordFieldComponent } from '../../components/password-field/password-field.component';
import { EmailFieldComponent } from '../../components/email-field/email-field.component';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [MaterialModule, FormsModule, CommonModule, MatCheckboxModule, PasswordFieldComponent, EmailFieldComponent],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  hide = true;
  remember: boolean = true
  email: string = '';
  password: string = '';
  errorMessage: string | null = null;
  constructor(private authService: AuthService, private router: Router, private dialogue: MatDialog) {}
  async login() {
    const success = await this.authService.login(this.email, this.password);
    if (!success) this.errorMessage = 'login failed';
    else {
      this.router.navigate(['']);
      this.errorMessage = null;
    }
    // console.log(this.authService.currentUserValue);
  }

  popup(){
    this.dialogue.open(ForgotPasswordPopupComponent, { data: this.email.value, autoFocus: false })
  }
}
