import { Component } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ClearableInputComponent } from '../../components/clearable-input/clearable-input.component';
import { EmailFieldComponent } from '../../components/email-field/email-field.component';
import { SelectComponent } from '../../components/select/select.component';
@Component({
  selector: 'app-invite-modal',
  standalone: true,
  imports: [MaterialModule, FormsModule, CommonModule, ClearableInputComponent, EmailFieldComponent, MaterialModule, SelectComponent],
  templateUrl: './invite-modal.component.html',
  styleUrl: './invite-modal.component.css'
})
export class InviteModalComponent {
  /**
   * placeholder for API values
   */
  roles = [
    {value: "projectOwner", viewValue: "Project Owner"},
    {value: "developer", viewValue: "Developer"},
    {value: "manager", viewValue: "Manager"},
  ];
  email: string = '';
  firstName: string = '';
  lastName: string = '';
  role: string | null = null;
  errorMessage: string | null = null;

  // query params
  // token: string | undefined;
  // email: string | undefined;
  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}
  // listen to query parameters
  // ngOnInit() {
  //   this.route.queryParams.subscribe((params) => {
  //     this.token = params['token'];
  //     this.email = params['email'];
  //   });
  // }
  async register() {
    // check token
    // if (this.token === undefined) {
    //   this.errorMessage = 'no token';
    //   return;
    // }
    // check email
    if (this.email === undefined) {
      this.errorMessage = 'no email passed';
      return;
    }
    // check if password confirm matches
    // if (this.password !== this.passwordConfirm) {
    //   this.errorMessage = 'passwords do not match!';
    //   return;
    // }
    const res = await this.authService.register(
      this.email,
      this.firstName,
      this.lastName
    );
    // if registration fails return
    if (!res) {
      this.errorMessage = 'registration failed';
      return;
    }
    // on successful registration, redirect to home?
    this.router.navigate(['/login']);
  }
}
