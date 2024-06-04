import { Component, EventEmitter, Inject, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { LocalStorageService } from '../services/localstorage';
import { GanttType } from '../components/ngxgantt/ngxgantt.component';
import { NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-ngxgantt-columns-modal',
  standalone: true,
  imports: [ MatButtonModule, NgIf, FormsModule ],
  templateUrl: './ngxgantt-columns-modal.component.html',
  styleUrl: './ngxgantt-columns-modal.component.css'
})
export class NgxganttColumnsModalComponent {
  constructor(
    public dialogRef: MatDialogRef<NgxganttColumnsModalComponent>,
    public localStorage: LocalStorageService,
    @Inject(MAT_DIALOG_DATA) public data : any) {
      this.ganttType = this.data.ganttType;
      this.localStorageColumnName = this.data.localStorageColumnName;
      this.columns = this.localStorage.getData(this.localStorageColumnName!);
      console.log(this.localStorageColumnName);
  }
  @Output() notifyUpdate: EventEmitter<void> = new EventEmitter<void>();

  ganttType?: GanttType;
  localStorageColumnName?: string;

  columns?: { name: boolean, startDate: boolean, endDate: boolean, status?: boolean, priority?: boolean, users?: boolean};

  closeDialog() {
    this.dialogRef.close();
  }

  toggle(option: string, event: Event) {
    const inputElement = event.target as HTMLInputElement;
    const isChecked = inputElement.checked;

    switch(option) {
      case "name":
        this.columns!.name = isChecked;
        break;
      case "startDate":
        this.columns!.startDate = isChecked;
        break;
      case "endDate":
        this.columns!.endDate = isChecked;
        break;
      case "status":
        this.columns!.status = isChecked;
        break;
      case "priority":
        this.columns!.priority = isChecked;
        break;
      case "users":
        this.columns!.users = isChecked;
        break;
    }
    this.localStorage.saveData(this.localStorageColumnName!, this.columns);

    this.notifyUpdate.emit();
  }
}
