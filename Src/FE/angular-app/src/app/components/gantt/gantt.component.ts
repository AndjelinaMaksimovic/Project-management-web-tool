import { Component, Input, OnInit } from '@angular/core';
import { GanttColumn, Item, Items, TimeScale } from './item';
import { formatDate, NgClass, NgIf, NgStyle } from '@angular/common';
import { Task } from '../../services/task.service';

@Component({
  selector: 'app-gantt',
  standalone: true,
  imports: [ NgStyle, NgIf, NgClass ],
  templateUrl: './gantt.component.html',
  styleUrl: './gantt.component.css'
})

export class GanttComponent implements OnInit{
  @Input() tasks: Task[] = []
  items: Items = []
  @Input() columns: GanttColumn[] = [GanttColumn.tasks]
  @Input() colWidths: number[] = [100]
  @Input() timeScale: TimeScale = TimeScale.day
  @Input() holidays: Date[] = []

  dates!: string[]
  chartStartDate!: number

  columnWidth = 60
  taskHeight = 20
  barHeight = 14

  GanttColumn = GanttColumn // must be declared to be used in html

  ngOnInit(): void {
    if(this.tasks.length==0){
      this.dates = []
      this.chartStartDate = Date.now()
      return
    }

    this.items = new Array<Item>(this.tasks.length)
    for (let i = 0; i < this.items.length; i++) {
      this.items[i] = new Item(
        this.tasks[i].id,
        this.tasks[i].title,
        this.tasks[i].description,
        this.tasks[i].category,
        this.tasks[i].priority,
        this.tasks[i].status,
        Date.now(),
        this.tasks[i].date.valueOf(),
        this.tasks[i].assignedTo
      )
    }

    this.initTimeHeader()

    this.holidays.forEach(date => {
      date.setHours(0, 0, 0, 0)
    });

    this.items.forEach(item => {
      var t = Math.floor((item.startDate - this.chartStartDate) / this.timeScale)
      t = t - this.range(this.chartStartDate, item.startDate, this.timeScale).reduce((prev, curr) => this.includeDay(curr) ? prev : prev + 1, 0)
      item.left = t*this.columnWidth + 'px'
      // console.log()
      t = item.startDate - item.startDate % this.timeScale  // normalize start
      t = (Math.ceil((item.dueDate - t) / this.timeScale))
      t = t - this.range(item.startDate, item.dueDate, this.timeScale).reduce((prev, curr) => this.includeDay(curr) ? prev : prev + 1, 0)
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
    const max = this.items.reduce((a, b)=>{return a.dueDate > b.dueDate ? a : b}).dueDate
    const min = this.items.reduce((a, b)=>{return a.startDate < b.startDate ? a : b}).startDate
    this.chartStartDate = min - min % this.timeScale
    this.dates = this.range(this.chartStartDate, max, this.timeScale) // example: max is friday 5 pm, adds friday 00:00 so no need to round up
      .filter((v, i) => {
        return this.includeDay(v) // remove weekend and holiday
      })
      .map((v) => {
        // const format = this.timeScale == TimeScale.day ? "d EEEEE" : "d HH"
        const format = this.timeScale == TimeScale.day ? "d. E" : "d HH"
        return formatDate(v, format, "en-US") // day starts at UTC but displays in local timezone, could cause weird offset?
      })
  }

  rowHover(){

  }
}
