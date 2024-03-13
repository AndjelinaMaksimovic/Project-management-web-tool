import { Component } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
@Component({
  selector: 'app-register',
  standalone: true,
  imports: [MaterialModule, FormsModule, CommonModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  password: string = '';
  passwordConfirm: string = '';
  errorMessage: string | null = null;

  // query params
  token: string | undefined;
  email: string | undefined;
  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}
  // listen to query parameters
  ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      this.token = params['token'];
      this.email = params['email'];
    });
  }
  async register() {
    // check token
    if (this.token === undefined) {
      this.errorMessage = 'no token';
      return;
    }
    // check email
    if (this.email === undefined) {
      this.errorMessage = 'no email passed';
      return;
    }
    // check if password confirm matches
    if (this.password !== this.passwordConfirm) {
      this.errorMessage = 'passwords do not match!';
      return;
    }
    const res = await this.authService.register(
      this.token,
      this.email,
      this.password
    );
    // if registration fails return
    if (!res) {
      this.errorMessage = 'registration failed';
      return;
    }
    // on successful registration, redirect to home?
    this.router.navigate(['/home']);
  }
}
