import { Component } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [MaterialModule, FormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  email: string = '';
  password: string = '';
  errorMessage: string | null = null;
  constructor(private authService: AuthService, private router: Router) {}
  async login() {
    const success = await this.authService.login(this.email, this.password);
    if (!success) this.errorMessage = 'login failed';
    else {
      this.router.navigate(['']);
      this.errorMessage = null;
    }
    // console.log(this.authService.currentUserValue);
  }
}
