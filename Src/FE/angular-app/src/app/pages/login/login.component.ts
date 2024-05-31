import { Component } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { AuthService } from '../../services/auth.service';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
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
  imports: [MaterialModule, FormsModule, CommonModule, MatCheckboxModule, PasswordFieldComponent, EmailFieldComponent, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  hide = true;
  remember: boolean = true
  password = new FormControl('', [Validators.required]);
  email = new FormControl('', [Validators.email, Validators.required]);
  errorMessage: string | null = null;
  constructor(private authService: AuthService, private router: Router, private dialogue: MatDialog) {}
  async login() {
    this.email.markAsTouched()
    this.password.markAsTouched()
    if(
      !this.email.value
      || !this.password.value
      || this.email.errors
      || this.password.errors
    ){
      return
    }
    const success = await this.authService.login(this.email.value, this.password.value);
    if (!success) this.errorMessage = 'Bad username or password';
    else {
      this.router.navigate(['']);
      this.errorMessage = null;
    }
    // console.log(this.authService.currentUserValue);
  }

  popup(){
    this.dialogue.open(ForgotPasswordPopupComponent, { data: this.email, autoFocus: false })
  }
}
