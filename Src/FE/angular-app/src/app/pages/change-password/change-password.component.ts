import { Component, HostBinding, HostListener, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MaterialModule } from '../../material/material.module';
import { AbstractControl, FormControl, FormsModule, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { PasswordFieldComponent } from '../../components/password-field/password-field.component';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [ MaterialModule, FormsModule, ReactiveFormsModule, CommonModule, PasswordFieldComponent ],
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.css'
})
export class ChangePasswordComponent implements OnInit {

  token?: string
  errorMessage?: string
  
  hide = true;
  password = new FormControl('', [Validators.required]);
  passwordConfirm = new FormControl('', [Validators.required, this.samePassword()]);

  constructor(private activatedRoute: ActivatedRoute, private userService: UserService, private router: Router){}

  ngOnInit() {
    this.activatedRoute.queryParams.subscribe((params) => {
      this.token = params['token'];
    });
  }

  @HostListener('document:keyup.enter')
  onEnter(){
    this.change()
  }

  async change(){
    this.password.markAsTouched()
    this.passwordConfirm.markAsTouched()
    if (this.token === undefined) {
      this.errorMessage = 'No token';
      return;
    }
    if (this.password === undefined) {
      this.errorMessage = 'Please add your new passwrod';
      return;
    }
    if(this.password.value != this.passwordConfirm.value){
      this.errorMessage = 'Passwords dont match'
      return;
    }
    if(!this.password.value){
      return
    }

    const res = await this.userService.updatePassword(this.token, this.password.value)
    if(res)
      this.router.navigate([''])
    else
      this.errorMessage = 'Server error'
  }

  samePassword(): ValidatorFn{
    return (control: AbstractControl) : ValidationErrors | null => {
      if(this.password.value && this.passwordConfirm.value && (this.password.value != this.passwordConfirm.value))
        return {differentPasswords: true}
      return null
    }
  }
}
