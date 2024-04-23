import { AfterViewInit, Component, ElementRef, HostListener, Input, OnInit, ViewChild } from '@angular/core';
import { DraggingType, GanttColumn, Item, ItemType, TimeScale } from './item';
import { formatDate, NgClass, NgIf, NgStyle } from '@angular/common';
import { Task } from '../../services/task.service';
import { GanttDependencyLineComponent } from './gantt-dependency-line/gantt-dependency-line.component';
import { MatDialog } from '@angular/material/dialog';
import { GanttSettingsComponent } from './gantt-settings/gantt-settings.component';
import { Subscription } from 'rxjs';
import { helpers } from './helpers';

@Component({
  selector: 'app-gantt',
  standalone: true,
  imports: [ NgStyle, NgIf, NgClass, GanttDependencyLineComponent ],
  templateUrl: './gantt.component.html',
  styleUrl: './gantt.component.css'
})

export class GanttComponent implements OnInit, AfterViewInit{
  @Input() tasks: Task[] = []
  @Input() milestones: any[] = []
  @Input() items: Item[] = [] // only because task is readonly and missing gantt parameters like color
  @Input() columns: GanttColumn[] = [GanttColumn.tasks]
  @Input() colWidths: number[] = [100]
  @Input() timeScale: TimeScale = TimeScale.day
  
  @Input() hideWeekend: boolean = false
  @Input() holidays: Date[] = []

  // list of dates in the header
  dates!: string[]
  // marks the current date in the header
  currentDateIndex!: number
  chartStartDate!: number
  idMap: Record<number, number> = [] // id to index
  snapDate!: boolean

  columnWidth = 60
  taskHeight = 20
  barHeight = 14

  GanttColumn = GanttColumn // must be declared to be used in html
  ItemType = ItemType // must be declared to be used in html
  helpers = helpers
  DraggingType = DraggingType

  constructor(private dialogue: MatDialog){}

  ngOnInit(): void {
    if(this.items.length == 0){
      
      if(this.tasks.length == 0){ // no tasks or items provided
        this.dates = []
        this.idMap = []
        this.chartStartDate = Date.now()
        return
      }

      this.items = new Array<Item>(this.tasks.length + this.milestones.length)
      var i = 0
      for (; i < this.items.length; i++) {
        this.items[i] = new Item(
          this.tasks[i].id,
          this.tasks[i].title,
          this.tasks[i].description,
          this.tasks[i].category,
          this.tasks[i].priority,
          this.tasks[i].status,
          // this.tasks[i].startDate.valueOf(),
          (new Date("2024-04-06T00:00:00")).valueOf(),
          this.tasks[i].dueDate.valueOf(),
          this.tasks[i].assignedTo,
        )
        this.items[i].type = ItemType.task
      }
      for(let j = 0; j < this.milestones.length; j++, i++){
        this.items[i] = new Item(
          this.milestones[i].id,
          this.milestones[i].title,
          this.milestones[i].description,
          this.milestones[i].category,
        )
        this.items[i].startDate = this.milestones[i].startDate.valueOf()
        this.items[i].assignedTo = this.milestones[i].startDate
        this.items[i].type = ItemType.milestone
      }
    }
    this.sortByCategories()
    for (let i = 0; i < this.items.length; i++) { // init after sorting by categories
      this.idMap[this.items[i].id] = i
    }
    this.initTimeHeader()
    // this.holidays.forEach(date => {
    //   date.setHours(0, 0, 0, 0)
    // });
    this.initItemDisplay()
  }

  initTimeHeader(){
    const max = this.items.reduce((a, b)=>{return a.dueDate > b.dueDate ? a : b}).dueDate
    const min = this.items.reduce((a, b)=>{return a.startDate < b.startDate ? a : b}).startDate
    this.chartStartDate = min - min % this.timeScale /* for display ->*/ - this.timeScale * 1
    const datesNumber = helpers.range(this.chartStartDate, max /* for display ->*/ + this.timeScale * 20, this.timeScale) // example: max is friday 5 pm, adds friday 00:00 so no need to round up
    // this.dates = helpers.range(this.chartStartDate, max, this.timeScale) // example: max is friday 5 pm, adds friday 00:00 so no need to round up
      .filter((v, i) => {
        return helpers.includeDay(v, this.hideWeekend, this.holidays) // remove weekend and holiday
      })
    this.currentDateIndex = datesNumber.indexOf(Date.now() - Date.now() % this.timeScale)
    this.dates = datesNumber.map((v) => {
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

  sortByCategories(){
    // sort categories by start date ------------------------------
    const categories = Object.entries(this.items.reduce<Record<string, {min: number, max: number}>>((prev, item) => {
      if(!prev[item.category]){
          prev[item.category] = {min: item.startDate, max: item.dueDate}
          return prev
      }
      if(prev[item.category].min > item.startDate)
          prev[item.category].min = item.startDate
      if(prev[item.category].max < item.dueDate)
          prev[item.category].max = item.dueDate
      return prev
    }, {}))
    const order = categories.sort((a,b)=> a[1].min - b[1].min).map((e, i)=>e[0])
    this.items.sort((a,b)=> order.indexOf(a.category) - order.indexOf(b.category) || a.startDate - b.startDate)
    // ---------------------------
    const newItem = new Item()
    newItem.title = categories[0][0]
    newItem.startDate = categories[0][1].min
    newItem.dueDate = categories[0][1].max
    newItem.color = 'grey'
    newItem.type = ItemType.category
    this.items.splice(0, 0, newItem)
    for(let i=2, j=1;i<this.items.length;i++){
      if(this.items[i].category != this.items[i-1].category){
        const newItem = new Item()
        newItem.title = categories[j][0]
        newItem.startDate = categories[j][1].min
        newItem.dueDate = categories[j][1].max
        newItem.color = 'grey'
        newItem.type = ItemType.category
        this.items.splice(i, 0, newItem)
        j+=1
        i+=1
      }
    }
  }

  initItemDisplay(){
    this.items.forEach(item => {
      this.snapDate = this.timeScale != TimeScale.week
      var t: number

      if(this.snapDate){
        t = Math.floor((item.startDate - this.chartStartDate) / this.timeScale)
        t = t - helpers.range(this.chartStartDate, item.startDate, this.timeScale).reduce((prev, curr) => helpers.includeDay(curr, this.hideWeekend, this.holidays) ? prev : prev + 1, 0)
        item.left = t*this.columnWidth
        t = item.startDate - item.startDate % this.timeScale  // normalize start
        t = (Math.ceil((item.dueDate - t) / this.timeScale))
        t = t - helpers.range(item.startDate, item.dueDate, this.timeScale).reduce((prev, curr) => helpers.includeDay(curr, this.hideWeekend, this.holidays) ? prev : prev + 1, 0)
        item.width = t*this.columnWidth
      }
      else{
        t = ((item.startDate - this.chartStartDate) / this.timeScale) * this.columnWidth
        t = t - helpers.range(this.chartStartDate, item.startDate, this.timeScale).reduce((prev, curr) => helpers.includeDay(curr, this.hideWeekend, this.holidays) ? prev : prev + 1, 0)
        item.left = t
        t = ((item.dueDate - item.startDate) / this.timeScale) * this.columnWidth
        t = t - helpers.range(item.startDate, item.dueDate, this.timeScale).reduce((prev, curr) => helpers.includeDay(curr, this.hideWeekend, this.holidays) ? prev : prev + 1, 0)
        item.width = t
      }
    });
  }

  lastHovered: Item = this.items.length > 0 ? this.items[0] : new Item() // quick hack
  dragging: DraggingType = DraggingType.none
  itemHover(item: Item){
    if(this.dragging == DraggingType.none){
      item.hover = true
    }
    this.lastHovered = item
  }
  itemUnHover(item: Item){
    if(this.dragging == DraggingType.none){
      item.hover = false
      // this.hovering = undefined
    }
  }
  clipLine = false
  barHover(item: Item){
    this.itemHover(item)
    if(this.dragging == DraggingType.dependency && this.originalItem != this.lastHovered && this.lastHovered.type != ItemType.category){
      this.clipLine = true
      this.offset = {x: this.lastHovered.left - this.draggedOriginal.x, y: this.idMap[this.lastHovered.id] * this.taskHeight + this.taskHeight / 2  - this.draggedOriginal.y}
    }
  }
  barUnHover(item: Item){
    this.clipLine = false
  }

  chartRect!: any
  @ViewChild('chartView', { static: false }) chartElem!: ElementRef;
  ngAfterViewInit(){
    this.chartRect = this.chartElem.nativeElement.getBoundingClientRect()
  }
  
  
  originalItem?: Item = undefined
  draggedOriginal: any = {x: 0, y: 0}
  offset: any = {x: 0, y: 0}

  startDependencyDrag(event: any) {
    this.dragging = DraggingType.dependency
    this.originalItem = this.lastHovered
    this.draggedOriginal = {x: this.lastHovered.left + this.lastHovered.width, y: this.idMap[this.lastHovered.id] * this.taskHeight + this.taskHeight / 2}
    this.onMouseMove({x: event.x, y: event.y})
    event.stopPropagation();
    return false
  }

  originalWidth = 0
  originalLeft = 0
  startTaskLeftEdgeDrag(event: any){
    this.dragging = DraggingType.taskEdgesLeft
    this.originalItem = this.lastHovered
    this.originalWidth = this.lastHovered.width
    this.originalLeft = this.lastHovered.left
    this.draggedOriginal = {x: event.x, y: event.y}
    event.stopPropagation();
    return false
  }
  startTaskRightEdgeDrag(event: any){
    this.dragging = DraggingType.taskEdgesRight
    this.originalItem = this.lastHovered
    this.originalWidth = this.lastHovered.width
    this.draggedOriginal = {x: event.x, y: event.y}
    event.stopPropagation();
    return false
  }

  startTaskDrag(event: any){
    if(this.lastHovered.type == ItemType.category)
      return false
    this.dragging = DraggingType.task
    this.originalItem = this.lastHovered
    this.originalLeft = this.lastHovered.left
    this.draggedOriginal = {x: event.x, y: event.y}
    event.stopPropagation();
    return false
  }

  @HostListener('mousemove', ['$event'])
  onMouseMove(event: MouseEvent | {x: number, y: number}){
    if(this.dragging == DraggingType.dependency && !this.clipLine)
      this.offset = {x: event.x - this.chartRect.left - this.draggedOriginal.x, y: event.y - this.chartRect.top - this.draggedOriginal.y}

    if(this.dragging == DraggingType.taskEdgesLeft && this.originalItem){
      this.originalItem.width = this.originalWidth - (event.x - this.draggedOriginal.x)
      this.originalItem.left = this.originalLeft + (event.x - this.draggedOriginal.x)
    }
    if(this.dragging == DraggingType.taskEdgesRight && this.originalItem){
      this.originalItem.width = this.originalWidth + event.x - this.draggedOriginal.x
    }

    if(this.dragging == DraggingType.task && this.originalItem){
      this.originalItem.left = this.originalLeft + (event.x - this.draggedOriginal.x)
    }
    return false
  }

  @HostListener('mouseup')
  onMouseUp() {
    if(this.dragging == DraggingType.dependency && this.originalItem){
      if(this.clipLine
        && this.lastHovered.id != this.originalItem?.id // not the same
        && !this.lastHovered.dependant.includes(this.originalItem.id) // last doesnt already contain it
        && !this.originalItem.dependant.includes(this.lastHovered.id)){ // current isn't dependant on it
        this.originalItem.dependant.push(this.lastHovered.id)
      }
      this.dragging = DraggingType.none
    }
    if(this.originalItem && this.originalItem != this.lastHovered)
      this.originalItem.hover = false

    if(this.dragging == DraggingType.taskEdgesLeft || this.dragging == DraggingType.taskEdgesRight){
      this.dragging = DraggingType.none
    }
    if(this.dragging == DraggingType.task){
      this.dragging = DraggingType.none
    }
    return false
  }

  @HostListener('mouseleave')
  onMouseLeave() {
    this.dragging = DraggingType.none
    this.lastHovered.hover = false
    if(this.originalItem)
      this.originalItem.hover = false
    return false
  }

  dependencyPopUp(item: Item, event: any){
    alert("clicked dep")
    event.stopPropagation()
    return false
  }
  editTask(item: Item){
    alert('edit task')
  }

  categoryToggle(itemIdx: number){
    itemIdx+=1
    const cat = this.items[itemIdx].category
    for(; itemIdx < this.items.length && this.items[itemIdx].category == cat; itemIdx++){
      this.items[itemIdx].display = !this.items[itemIdx].display
    }
  }

  openSettings(){
    const ref = this.dialogue.open(GanttSettingsComponent, { autoFocus: false, data: {
      scale: this.timeScale,
      hideWeekend: this.hideWeekend,
      holidays: this.holidays,
      columns: this.columns
    } })

    ref.beforeClosed().subscribe((data: any)=>{
      if(data){
        this.columns = data.columns
        if(this.timeScale != data.scale || this.hideWeekend != data.hideWeekend || this.holidays != data.holidays){
          this.timeScale = data.scale
          this.hideWeekend = data.hideWeekend
          this.holidays = data.holidays
          this.initTimeHeader()
          this.initItemDisplay()
        }
      }
    })
  }
}
