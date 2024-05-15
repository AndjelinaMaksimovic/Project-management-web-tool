import { Component, EventEmitter, HostListener, Inject, OnInit, Output } from '@angular/core';
import { MatSelectModule } from '@angular/material/select';
import { Column, GanttColumn, ItemSort, TimeScale } from '../item';
import { FormsModule } from '@angular/forms';
import { KeyValuePipe, NgFor } from '@angular/common';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MAT_DIALOG_DATA, MatDialogClose, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray } from '@angular/cdk/drag-drop';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import moment from 'moment';

@Component({
  selector: 'app-gantt-settings',
  standalone: true,
  imports: [
    MatSelectModule,
    MatCheckboxModule,
    MatButtonModule,
    FormsModule,
    NgFor,
    KeyValuePipe,
    CdkDropList,
    CdkDrag,
    MatIconModule,
    MatDialogClose,
    MatMenuModule
  ],
  templateUrl: './gantt-settings.component.html',
  styleUrl: './gantt-settings.component.css'
})
export class GanttSettingsComponent{
  scale: moment.unitOfTime.DurationConstructor
  itemSort: ItemSort
  hideWeekend: boolean
  groupByCategory: boolean
  holidays: Date[]
  columns: Column[]
  disableColumnsDrag: boolean = false

  // Object.keys on enum returns [..values, ..keys]. Why typescript?????
  timeScale = [
    {viewVal: 'Days', val: 'day'},
    {viewVal: 'Weeks', val: 'week'},
    {viewVal: 'Months', val: 'month'},
    {viewVal: 'Quarters', val: 'quarter'},
  ]
  itemSortArray = Object.values(ItemSort)
  hiddenColumns: Column[] = []

  //TODO: changing week to month -> infinite loop?

  constructor(@Inject(MAT_DIALOG_DATA) private data: any, private dialogRef: MatDialogRef<GanttSettingsComponent>){
    this.scale = this.data.scale
    this.itemSort = this.data.itemSort
    this.hideWeekend = this.data.hideWeekend
    this.groupByCategory = this.data.groupByCategory
    this.holidays = this.data.holidays
    this.columns = this.data.columns.map((col: Column) => ({...col})) // copy array
    this.hiddenColumns = 
      Object.values(GanttColumn)
      .filter(ent => !this.columns.find(col => col.type == ent))
      .map(obj => new Column(obj, 120))
  }

  colDrop(event: CdkDragDrop<string[]>){
    moveItemInArray(this.columns, event.previousIndex, event.currentIndex)
  }
  removeColumn(col: Column){
    this.columns.splice(this.columns.indexOf(col), 1)
    this.hiddenColumns.push(col)
  }
  addColumn(col: Column){
    this.columns.push(col)
    this.hiddenColumns.splice(this.hiddenColumns.indexOf(col), 1)
  }

  @HostListener('window:keyup.Enter', ['$event'])
  onDialogClick(event: KeyboardEvent): void {
    this.submit();
  }
  submit(){
    this.dialogRef.close({
      scale: this.scale,
      itemSort: this.itemSort,
      groupByCategory: this.groupByCategory,
      hideWeekend: this.hideWeekend,
      holidays: this.holidays,
      columns: this.columns,
    })
  }
  cancel(){
    this.dialogRef.close()
  }
}
