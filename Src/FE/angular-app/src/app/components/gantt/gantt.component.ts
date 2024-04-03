import { Component, Input, OnInit } from '@angular/core';
import { GanttColumn, Items, TimeScale } from './item';
import { formatDate, NgClass, NgIf, NgStyle } from '@angular/common';

@Component({
  selector: 'app-gantt',
  standalone: true,
  imports: [ NgStyle, NgIf, NgClass ],
  templateUrl: './gantt.component.html',
  styleUrl: './gantt.component.css'
})

export class GanttComponent implements OnInit{
  @Input() items: Items = []
  @Input() columns: GanttColumn[] = [GanttColumn.tasks]
  @Input() colWidths: number[] = [100]
  @Input() timeScale: TimeScale = TimeScale.day
  @Input() holidays: Date[] = []

  dates!: string[]
  chartStartDate!: number

  columnWidth = 40
  taskHeight = 20
  barHeight = 14

  GanttColumn = GanttColumn // must be declared to be used in html

  ngOnInit(): void {
    this.initTimeHeader()

    this.holidays.forEach(date => {
      date.setHours(0, 0, 0, 0)
    });

    this.items.forEach(item => {
      var t = Math.floor((item.start - this.chartStartDate) / this.timeScale)
      t = t - this.range(this.chartStartDate, item.start, this.timeScale).reduce((prev, curr) => this.includeDay(curr) ? prev : prev + 1, 0)
      item.left = t*this.columnWidth + 'px'
      // console.log()
      t = item.start - item.start % this.timeScale  // normalize start
      t = (Math.ceil((item.end - t) / this.timeScale))
      t = t - this.range(item.start, item.end, this.timeScale).reduce((prev, curr) => this.includeDay(curr) ? prev : prev + 1, 0)
      item.width = t*this.columnWidth + 'px'
    });
  }

  range(start: number, end: number, step: number = 1){ // inclusive
    return Array(Math.floor((end - start) / step + 1)).fill(0).map((_, i) => start + i * step)
  }
  colOffset(idx: number){
    return this.colWidths.slice(0, idx+1).reduce((a,b)=>a+b,0)
  }

  includeDay(day: number){
    const d = new Date(day);
    d.setHours(0, 0, 0, 0)
    day = d.getDay();
    if (day >= 1 && day <= 5 && !this.holidays.includes(d))
      return true
    return false
  }

  initTimeHeader(){
    const max = this.items.reduce((a, b)=>{return a.end > b.end ? a : b}).end
    const min = this.items.reduce((a, b)=>{return a.start < b.start ? a : b}).start
    this.chartStartDate = min - min % this.timeScale
    this.dates = this.range(this.chartStartDate, max, this.timeScale) // example: max is friday 5 pm, adds friday 00:00 so no need to round up
      .filter((v, i) => {
        return this.includeDay(v) // remove weekend and holiday
      })
      .map((v) => {
        const format = this.timeScale == TimeScale.day ? "d EEEEE" : "d HH"
        return formatDate(v, format, "en-US") // day starts at UTC but displays in local timezone, could cause weird offset?
      })
  }

  rowHover(){

  }
}
