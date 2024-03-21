import { Component } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ClearableInputComponent } from '../../components/clearable-input/clearable-input.component';
import { EmailFieldComponent } from '../../components/email-field/email-field.component';
import { SelectComponent } from '../../components/select/select.component';
import { RolesService } from '../../services/roles.service';
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
  roles: {value: string, viewValue: string}[] = [];
  email: string = '';
  firstName: string = '';
  lastName: string = '';
  role: string | null = null;
  errorMessage: string | null = null;

  constructor(
    private authService: AuthService,
    private rolesService: RolesService,
    private router: Router,
    private route: ActivatedRoute
  ) {}
  async ngOnInit(){
    const roles = await this.rolesService.getAllRoles();
    if(!roles) return;
    this.roles = roles.map(role => ({value: role.id.toString(), viewValue: role.roleName}))
  }
  async register() {
    // check email
    if (this.email === undefined) {
      this.errorMessage = 'no email passed';
      return;
    }
    if (this.role === null) {
      this.errorMessage = 'please select a role';
      return;
    }
    const res = await this.authService.register(
      this.email,
      this.firstName,
      this.lastName,
      this.role,
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
