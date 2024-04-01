import { Component, Input, OnInit } from '@angular/core';
import { GanttColumn, Items, TimeScale } from './item';
import { formatDate, NgStyle } from '@angular/common';

@Component({
  selector: 'app-gantt',
  standalone: true,
  imports: [ NgStyle ],
  templateUrl: './gantt.component.html',
  styleUrl: './gantt.component.css'
})

export class GanttComponent implements OnInit{
  @Input() items: Items = []
  @Input() columns: GanttColumn[] = [GanttColumn.title]
  @Input() timeScale: TimeScale = TimeScale.day
  
  dates!: string[]
  chartStartDate!: number

  constructor(){
  }

  ngOnInit(): void {
    this.initTimeHeader()

    this.items.forEach(item => {
      item.left = Math.floor((item.start - this.chartStartDate) / this.timeScale)*50 + 'px'
      item.width = (Math.floor((item.end - item.start) / this.timeScale)+1)*50 + 'px'

      item.left = Math.floor((item.start - this.chartStartDate) / this.timeScale)*50 + 'px'
      item.width = (Math.floor((item.end - item.start) / this.timeScale)+1)*50 + 'px'
    });
  }

  includeDay(day: number, holidays: Date[]){
    const d = new Date(day);
    day = d.getDay();
    if (day >= 1 && day <= 5 && !holidays.includes(d))
      return true
    return false
  }

  initTimeHeader(){
    const max = this.items.reduce((a, b)=>{return a.end > b.end ? a : b}).end
    const min = this.items.reduce((a, b)=>{return a.start < b.start ? a : b}).start
    const len = Math.ceil((max - min) / this.timeScale) + 1
    this.chartStartDate = min - min % this.timeScale
    this.dates = Array(len)
      .fill(this.chartStartDate)
      .map((v, i) => {
        return v + i*this.timeScale // dates = range(project_start, project_end, timeScale)
      })
      .map((v) => {
        const format = this.timeScale == TimeScale.day ? "d EEEEE" : "d:h"
        return formatDate(v, format, "en-US") // bug? day starts at UTC (+1 for serbia), should start at local timezone
      })
  }
}
