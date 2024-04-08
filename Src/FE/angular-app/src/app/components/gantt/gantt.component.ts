import { Component, Input, OnInit } from '@angular/core';
import { GanttColumn, Item, TimeScale } from './item';
import { formatDate, NgClass, NgIf, NgStyle } from '@angular/common';
import { Task } from '../../services/task.service';

// TODO: bug when item width is 0 or starts and ends during the weekend

@Component({
  selector: 'app-gantt',
  standalone: true,
  imports: [ NgStyle, NgIf, NgClass ],
  templateUrl: './gantt.component.html',
  styleUrl: './gantt.component.css'
})

export class GanttComponent implements OnInit{
  @Input() tasks: Task[] = []
  @Input() items: Item[] = []
  @Input() columns: GanttColumn[] = [GanttColumn.tasks]
  @Input() colWidths: number[] = [100]
  @Input() timeScale: TimeScale = TimeScale.day
  
  @Input() hideWeekend: boolean = false
  @Input() holidays: Date[] = []

  dates!: string[]
  chartStartDate!: number
  idMap: Record<number, number> = [] // id to index

  columnWidth = 60
  taskHeight = 20
  barHeight = 14

  GanttColumn = GanttColumn // must be declared to be used in html

  ngOnInit(): void {
    if(this.items.length == 0){
      
      if(this.tasks.length == 0){ // no tasks or items provided
        this.dates = []
        this.idMap = []
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
          // this.tasks[i].startDate.valueOf(),
          Date.now(),
          this.tasks[i].dueDate.valueOf(),
          this.tasks[i].assignedTo
        )
      }
    }
    for (let i = 0; i < this.items.length; i++) {
      this.idMap[this.items[i].id] = i
    }

    this.initTimeHeader()
    this.holidays.forEach(date => {
      date.setHours(0, 0, 0, 0)
    });

    this.items.forEach(item => {
      var t = Math.floor((item.startDate - this.chartStartDate) / this.timeScale)
      t = t - this.range(this.chartStartDate, item.startDate, this.timeScale).reduce((prev, curr) => this.includeDay(curr) ? prev : prev + 1, 0)
      item.left = t*this.columnWidth
      // console.log()
      t = item.startDate - item.startDate % this.timeScale  // normalize start
      t = (Math.ceil((item.dueDate - t) / this.timeScale))
      t = t - this.range(item.startDate, item.dueDate, this.timeScale).reduce((prev, curr) => this.includeDay(curr) ? prev : prev + 1, 0)
      item.width = t*this.columnWidth
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
    if (this.hideWeekend && (day == 0 || day == 6))
      return false
    if(this.holidays.includes(d))
      return false
    return true
  }

  initTimeHeader(){
    // this.items.forEach(e => {
    //   console.log("start: " + new Date(e.startDate) + "; due: " + new Date(e.dueDate))
    // })
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
}
