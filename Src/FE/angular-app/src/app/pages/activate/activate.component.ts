import { Component } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { PasswordFieldComponent } from '../../components/password-field/password-field.component';

@Component({
  selector: 'app-activate',
  standalone: true,
  imports: [MaterialModule, FormsModule, CommonModule, PasswordFieldComponent],
  templateUrl: './activate.component.html',
  styleUrl: './activate.component.css',
})
export class ActivateComponent {
  password: string = '';
  passwordConfirm: string = '';
  errorMessage: string | null = null;

  handlePasswordChange(password: string){
    this.password = password;
  }
  handlePasswordConfirmChange(passwordConfirm: string){
    this.passwordConfirm = passwordConfirm;
  }
  // query params
  token: string | undefined;
  email: string | undefined;

  activated?: boolean = undefined;

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}
  // listen to query parameters
  async ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      this.token = params['token'];
      this.email = params['email'];
    });

    const res = await this.authService.check(
      this.token!,
      this.email!
    );
    this.activated = !res;
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
    await this.authService.logout();
    const res = await this.authService.activate(
      this.token,
      this.email,
      this.password
    );
    // if registration fails return
    if (!res) {
      this.errorMessage = 'activation failed';
      return;
    }
    // on successful registration, redirect to home?
    this.router.navigate(['']);
  }

  registerButton() {
    this.router.navigate(['/register']);
  }
  
  loginButton() {
    this.router.navigate(['/login']);
  }
}
