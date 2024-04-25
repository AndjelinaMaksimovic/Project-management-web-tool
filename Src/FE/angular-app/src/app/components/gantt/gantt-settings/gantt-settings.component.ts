import { Component, EventEmitter, Inject, OnInit, Output } from '@angular/core';
import { MatSelectModule } from '@angular/material/select';
import { GanttColumn, TimeScale } from '../item';
import { FormsModule } from '@angular/forms';
import { KeyValuePipe, NgFor } from '@angular/common';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-gantt-settings',
  standalone: true,
  imports: [ MatSelectModule, MatCheckboxModule, MatButtonModule, FormsModule, NgFor, KeyValuePipe ],
  templateUrl: './gantt-settings.component.html',
  styleUrl: './gantt-settings.component.css'
})
export class GanttSettingsComponent{
  scale: string
  hideWeekend: boolean
  holidays: Date[]
  columns: GanttColumn[]

  timeScale = Object.keys(TimeScale).slice(Object.keys(TimeScale).length / 2) // Object.keys returns [..values, ..keys]. Why typescript?????
  columnsEnum = Object.keys(GanttColumn).slice(Object.keys(GanttColumn).length / 2) // Object.keys returns [..values, ..keys]. Why typescript?????

  constructor(@Inject(MAT_DIALOG_DATA) private data: any, private dialogRef: MatDialogRef<GanttSettingsComponent>){
    this.scale = TimeScale[this.data.scale]
    this.hideWeekend = this.data.hideWeekend
    this.holidays = this.data.holidays
    this.columns = this.data.columns
  }
  submit(){
    this.dialogRef.close({
      scale: (TimeScale as any)[this.scale],
      hideWeekend: this.hideWeekend,
      holidays: this.holidays,
      columns: this.columns,
    })
  }
  cancel(){
    this.dialogRef.close()
  }
}
