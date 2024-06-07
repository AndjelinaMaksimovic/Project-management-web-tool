import { Component, Inject } from '@angular/core';
import { FormControl, FormGroupDirective, FormsModule, NgForm, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { ErrorStateMatcher } from '@angular/material/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { RolesService } from '../../services/roles.service';
import { MatSelectModule } from '@angular/material/select';
import { InviteService } from '../../services/invite.service';

export class MyErrorStateMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    const isSubmitted = form && form.submitted;
    return !!(control && control.invalid && (control.dirty || control.touched || isSubmitted));
  }
}

@Component({
  selector: 'app-new-member-modal',
  standalone: true,
  imports: [ MatDialogModule, MatButtonModule, MatInputModule, FormsModule, MatFormFieldModule, ReactiveFormsModule, MatSelectModule ],
  templateUrl: './new-member-modal.component.html',
  styleUrl: './new-member-modal.component.css'
})
export class NewMemberModalComponent {
  emailFormControl = new FormControl('', [Validators.required, Validators.email]);
  firstnameFormControl = new FormControl('', [Validators.required]);
  lastnameFormControl = new FormControl('', [Validators.required]);
  roleFormControl = new FormControl('', [Validators.required]);
  matcher = new MyErrorStateMatcher();

  constructor(
    public dialogRef: MatDialogRef<NewMemberModalComponent>,
    private roleService: RolesService,
    private inviteService: InviteService,
    @Inject(MAT_DIALOG_DATA) public data : any) {
  }

  roles: { roleName: string, id: number }[] | null = [];

  async ngOnInit() {
    this.roles = await this.roleService.getAllRoles();
  }

  onSubmit() {
    if (this.firstnameFormControl.valid && this.lastnameFormControl && this.emailFormControl.valid && this.roleFormControl) {
      this.inviteService.inviteUser(this.firstnameFormControl.value!, this.lastnameFormControl.value!, this.emailFormControl.value!, "", parseInt(this.roleFormControl.value!));
      this.closeDialog();
    } else {
      
    }
  }

  closeDialog() {
    this.dialogRef.close();
  }
}
