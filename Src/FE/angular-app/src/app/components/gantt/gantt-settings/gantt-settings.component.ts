import { Component, EventEmitter, Inject, OnInit, Output } from '@angular/core';
import { MatSelectModule } from '@angular/material/select';
import { Column, GanttColumn, TimeScale } from '../item';
import { FormsModule } from '@angular/forms';
import { KeyValuePipe, NgFor } from '@angular/common';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray } from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-gantt-settings',
  standalone: true,
  imports: [ MatSelectModule, MatCheckboxModule, MatButtonModule, FormsModule, NgFor, KeyValuePipe, CdkDropList, CdkDrag ],
  templateUrl: './gantt-settings.component.html',
  styleUrl: './gantt-settings.component.css'
})
export class GanttSettingsComponent{
  scale: string
  hideWeekend: boolean
  holidays: Date[]
  columns: Column[]
  disableColumnsDrag: boolean = false

  timeScale = Object.keys(TimeScale).filter(key => isNaN(Number(key))) // Object.keys returns [..values, ..keys]. Why typescript?????

  //TODO: changing week to month -> infinite loop?

  constructor(@Inject(MAT_DIALOG_DATA) private data: any, private dialogRef: MatDialogRef<GanttSettingsComponent>){
    this.scale = TimeScale[this.data.scale]
    this.hideWeekend = this.data.hideWeekend
    this.holidays = this.data.holidays
    this.columns = this.data.columns.map((col: Column) => ({...col})) // copy array
  }

  colDrop(event: CdkDragDrop<string[]>){
    moveItemInArray(this.columns, event.previousIndex, event.currentIndex)
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
