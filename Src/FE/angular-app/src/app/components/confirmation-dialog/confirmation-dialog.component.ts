import { Component, Inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-confirmation-dialog',
  standalone: true,
  imports: [ MatDialogModule, MatButtonModule ],
  templateUrl: './confirmation-dialog.component.html',
  styleUrl: './confirmation-dialog.component.css'
})
export class ConfirmationDialogComponent {
  title: string = "";
  description: string = "";

  yesFunc: () => void = () => {};
  noFunc: () => void = () => {};

  constructor(
    public dialogRef: MatDialogRef<ConfirmationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { title: string, description: string, yesFunc: () => void, noFunc: () => void }) {
    this.title = data.title;
    this.description = data.description;
    this.yesFunc = data.yesFunc;
    this.noFunc = data.noFunc;
  }

  closeDialog() {
    this.dialogRef.close();
  }
}
