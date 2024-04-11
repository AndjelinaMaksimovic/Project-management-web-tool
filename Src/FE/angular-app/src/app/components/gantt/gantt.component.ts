import { AfterViewInit, Component, ElementRef, HostListener, Input, OnInit, ViewChild } from '@angular/core';
import { GanttColumn, Item, Milestone, TimeScale } from './item';
import { formatDate, NgClass, NgIf, NgStyle } from '@angular/common';
import { Task } from '../../services/task.service';
import { GanttDependencyLineComponent } from '../gantt-dependency-line/gantt-dependency-line.component';

@Component({
  selector: 'app-gantt',
  standalone: true,
  imports: [ NgStyle, NgIf, NgClass, GanttDependencyLineComponent ],
  templateUrl: './gantt.component.html',
  styleUrl: './gantt.component.css'
})

export class GanttComponent implements OnInit, AfterViewInit{
  @Input() tasks: Task[] = []
  @Input() items: Item[] = [] // only because task is readonly and missing gantt parameters like color
  @Input() milestones: Milestone[] = []
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
      var t: number
      // t = Math.floor((item.startDate - this.chartStartDate) / this.timeScale)
      // t = t - this.range(this.chartStartDate, item.startDate, this.timeScale).reduce((prev, curr) => this.includeDay(curr) ? prev : prev + 1, 0)
      // item.left = t*this.columnWidth
      // t = item.startDate - item.startDate % this.timeScale  // normalize start
      // t = (Math.ceil((item.dueDate - t) / this.timeScale))
      // t = t - this.range(item.startDate, item.dueDate, this.timeScale).reduce((prev, curr) => this.includeDay(curr) ? prev : prev + 1, 0)
      // item.width = t*this.columnWidth
      t = ((item.startDate - this.chartStartDate) / this.timeScale) * this.columnWidth
      t = t - this.range(this.chartStartDate, item.startDate, this.timeScale).reduce((prev, curr) => this.includeDay(curr) ? prev : prev + 1, 0)
      item.left = t
      t = ((item.dueDate - item.startDate) / this.timeScale) * this.columnWidth
      t = t - this.range(item.startDate, item.dueDate, this.timeScale).reduce((prev, curr) => this.includeDay(curr) ? prev : prev + 1, 0)
      item.width = t
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
    const max = this.items.reduce((a, b)=>{return a.dueDate > b.dueDate ? a : b}).dueDate
    const min = this.items.reduce((a, b)=>{return a.startDate < b.startDate ? a : b}).startDate
    this.chartStartDate = min - min % this.timeScale
    this.dates = this.range(this.chartStartDate, max, this.timeScale) // example: max is friday 5 pm, adds friday 00:00 so no need to round up
      .filter((v, i) => {
        return this.includeDay(v) // remove weekend and holiday
      })
      .map((v) => {
        var format: string
        switch (this.timeScale) {
          case TimeScale.week:
            format = "d. E"
            break;
          case TimeScale.day:
            format = "d. E"
            break;
          case TimeScale.hour:
            format = "d. HH:00"
            break;
          default:
            format = "d. E"
            break;
        }
        // const format = this.timeScale == TimeScale.day ? "d. E" : "d HH"
        return formatDate(v, format, "en-US") // day starts at UTC but displays in local timezone, could cause weird offset?
      })
  }

  lastHovered!: Item | Milestone
  dragging: boolean = false
  itemHover(item: Item | Milestone){
    if(!this.dragging){
      item.hover = true
    }
    this.lastHovered = item
  }
  itemUnHover(item: Item | Milestone){
    if(!this.dragging){
      item.hover = false
      // this.hovering = undefined
    }
  }
  clipLine = false
  barHover(item: Item | Milestone){
    this.itemHover(item)
    if(this.dragging && this.originalItem != this.lastHovered){
      this.clipLine = true
      this.offset = {x: this.lastHovered.left - this.draggedOriginal.x + 5, y: this.idMap[this.lastHovered.id] * this.taskHeight + this.taskHeight / 2  - this.draggedOriginal.y}
    }
  }
  barUnHover(item: Item | Milestone){
    this.clipLine = false
  }

  chartRect!: any
  @ViewChild('chartView', { static: false }) chartElem!: ElementRef;
  ngAfterViewInit(){
    this.chartRect = this.chartElem.nativeElement.getBoundingClientRect()
  }
  
  originalItem?: Item | Milestone = undefined
  draggedOriginal: any = {x: 0, y: 0}
  offset: any = {x: 0, y: 0}

  @HostListener('mousedown', ['$event'])
  onMouseDown(event: any) {
    console.log(event.x +" "+ event.y)
    this.dragging = true
    this.originalItem = this.lastHovered
    this.draggedOriginal = {x: this.lastHovered.left + this.lastHovered.width - 5, y: this.idMap[this.lastHovered.id] * this.taskHeight + this.taskHeight / 2}
    this.onMouseMove({x: event.x, y: event.y})
    return false
  }
  @HostListener('mousemove', ['$event'])
  onMouseMove(event: any){
    if(this.dragging && !this.clipLine)
      this.offset = {x: event.x - this.chartRect.left - this.draggedOriginal.x, y: event.y - this.chartRect.top - this.draggedOriginal.y}
    return false
  }

  @HostListener('mouseup')
  onMouseUp(target: any) {
    if(this.dragging){
      if(this.clipLine && !this.lastHovered.dependant.includes(this.lastHovered.id) && this.lastHovered.id != this.originalItem?.id){
        this.originalItem?.dependant.push(this.lastHovered.id)
      }
      this.dragging = false
    }
    if(this.originalItem && this.originalItem != this.lastHovered)
      this.originalItem.hover = false
    return false
  }

  @HostListener('mouseleave')
  onMouseLeave() {
    this.dragging = false
    this.lastHovered.hover = false
    if(this.originalItem)
      this.originalItem.hover = false
    return false
  }
}
