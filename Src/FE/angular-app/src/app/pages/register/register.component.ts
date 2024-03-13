import { Component } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
@Component({
  selector: 'app-register',
  standalone: true,
  imports: [MaterialModule, FormsModule, CommonModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  email: string = '';
  password: string = '';
  passwordConfirm: string = '';
  errorMessage: string | null = null;
  constructor(private authService: AuthService, private router: Router) {}
  async register() {
    console.log('register');
    const res = await this.authService.register('test', 'test', '1234');
    // if registration fails return
    if (!res) {
      console.log('registration failed');
      this.errorMessage = 'registration failed';
      return;
    }
    // on successful registration, redirect to home?
    this.router.navigate(['/home']);
  }
}
