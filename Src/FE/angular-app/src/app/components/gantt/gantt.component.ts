import { AfterViewInit, Component, ElementRef, HostListener, Input, OnInit, ViewChild } from '@angular/core';
import { Column, DraggingType, GanttColumn, Item, ItemType, TimeScale, ItemSort, Category } from './item';
import { formatDate, NgClass, NgIf, NgStyle } from '@angular/common';
import { Task, TaskService } from '../../services/task.service';
import { GanttDependencyLineComponent } from './gantt-dependency-line/gantt-dependency-line.component';
import { MatDialog } from '@angular/material/dialog';
import { GanttSettingsComponent } from './gantt-settings/gantt-settings.component';
import { Subscription } from 'rxjs';
import { helpers } from './helpers';
import { Route, Router, RouterModule } from '@angular/router';
import { CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray } from '@angular/cdk/drag-drop';
import { MatIconModule } from '@angular/material/icon';
import { CategoryService } from '../../services/category.service';
import moment, { unitOfTime } from 'moment';

@Component({
  selector: 'app-gantt',
  standalone: true,
  imports: [
    NgStyle,
    NgIf,
    NgClass,
    GanttDependencyLineComponent,
    RouterModule,
    CdkDropList,
    CdkDrag,
    MatIconModule,
  ],
  templateUrl: './gantt.component.html',
  styleUrl: './gantt.component.css'
})

export class GanttComponent implements OnInit, AfterViewInit{
  @Input() tasks: Task[] = []
  @Input() milestones: any[] = []
  @Input() items: Item[] = [] // only because task is readonly and missing gantt parameters like color
  @Input() columns: Column[] = [new Column(GanttColumn.tasks, 180)]
  @Input() timeScale: moment.unitOfTime.DurationConstructor = 'day'
  itemSort: ItemSort = ItemSort.custom
  categories: Category[] = []
  groupByCategory: boolean = true
  
  @Input() hideWeekend: boolean = false
  @Input() holidays: Date[] = []

  // list of dates in the header
  secondaryDates: {value: string, len: number}[] = []
  dates: string[] = []
  // marks the current date in the header
  currentDateIndex!: number
  chartStartDate!: number
  idMap: Record<number, number> = [] // id to index
  snapDate: boolean = true
  // allCategories: {name: string, idx: number}[] = []

  columnWidth = 25
  minColumnWidth = 20
  taskHeight = 20
  barHeight = 16

  GanttColumn = GanttColumn // must be declared to be used in html
  ItemType = ItemType // must be declared to be used in html
  helpers = helpers
  DraggingType = DraggingType
  ItemSort = ItemSort
  moment = moment

  priorityToColor = {'Low': '#03fc03', 'Medium': '#fcf803', 'High': '#fc1c03'}

  constructor(private dialogue: MatDialog, private router: Router, private categoryService: CategoryService, private taskService: TaskService){}

  async ngOnInit() {
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
          // this.tasks[i].index,
          // this.tasks[i].indexInCategory,
          0,
          0,
          this.tasks[i].projectId,
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
        this.items[i].color = this.priorityToColor[this.items[i].priority]
        this.items[i].type = ItemType.task
      }
      for(let j = 0; j < this.milestones.length; j++, i++){
        this.items[i] = new Item(
          this.milestones[i].id,
          // this.milestones[i].index,
          // this.tasks[i].indexInCategory,
          0,
          0,
          this.milestones[i].projectId,
          this.milestones[i].title,
          this.milestones[i].description,
          this.milestones[i].category,
        )
        this.items[i].startDate = this.milestones[i].startDate.valueOf()
        this.items[i].assignedTo = this.milestones[i].startDate
        this.items[i].color = this.priorityToColor[this.items[i].priority]
        this.items[i].type = ItemType.milestone
      }
    }

    // TODO: test
    // const limit = 50
    // this.items = []
    // for (let i = 0; i < limit; i++) {
    //   const item = new Item(i, 1, "Item "+i, "desc", "category 1", "Low", "status 1", Date.now() + TimeScale.day*(i+3), Date.now()+TimeScale.day*(i+3), [{firstName: 'Milan', lastName: 'Milanovic', id: 1, profilePicture: ''}], (i==limit-1) ? [] : [i+1], 100, ItemType.task, '#5096A4')
    //   this.items.push(item)
    // }


    // TODO: multiple categories with same name. Include categoryId in item/task object
    // this.allCategories = this.categoryService.getCategories().map(cat => {return {name: cat.name, idx: cat.index}})
    // if(this.allCategories.length == 0){
      await this.categoryService.fetchCategories()
      this.categories = this.categoryService.getCategories().map(cat => new Category(cat.name, undefined, undefined, cat.index))
    // }
    // TODO: only for testing
    if(this.categories.length == 0)
      this.categories = [
        new Category("category 1", 0),
        new Category("category 2", 1),
        new Category("category 3", 2),
      ]

    if(this.groupByCategory){
      this.initCategories()
      this.insertCategories() // also init idMap
    } else{
      this.sortItems()
      this.updateIdMap()
    }

    this.initTimeHeader()
    // this.holidays.forEach(date => {
    //   date.setHours(0, 0, 0, 0)
    // });
    this.initItemDisplay()
  }

  initTimeHeader(){
    // const max = this.items.reduce((a, b)=>{return a.dueDate > b.dueDate ? a : b}).dueDate
    // const min = this.items.reduce((a, b)=>{return a.startDate < b.startDate ? a : b}).startDate
    // this.chartStartDate = min - min % this.timeScale /* for display ->*/ - this.timeScale * 1
    // const datesNumber = helpers.range(this.chartStartDate, max /* for display ->*/ + this.timeScale * 20, this.timeScale) // example: max is friday 5 pm, adds friday 00:00 so no need to round up
    // // this.dates = helpers.range(this.chartStartDate, max, this.timeScale) // example: max is friday 5 pm, adds friday 00:00 so no need to round up
    //   .filter((v, i) => {
    //     return helpers.includeDay(v, this.hideWeekend, this.holidays) // remove weekend and holiday
    //   })
    // this.currentDateIndex = datesNumber.indexOf(Date.now() - Date.now() % this.timeScale)
    // this.dates = datesNumber.map((v) => {
    //     var format: string
    //     switch (this.timeScale) {
    //       // case TimeScale.week:
    //       //   format = "d. E"
    //       //   break;
    //       // case TimeScale.day:
    //       //   format = "d. E"
    //       //   break;
    //       // case TimeScale.hour:
    //       //   format = "d. HH:00"
    //       //   break;
    //       default:
    //         format = "d. E"
    //         break;
    //     }
    //     return formatDate(v, format, "en-US") // day starts at UTC but displays in local timezone, could cause weird offset in TimeScale.hour?
    //   })
    this.dates = []
    this.secondaryDates = []
    const max = moment(this.items.reduce((a, b)=>{return a.dueDate > b.dueDate ? a : b}).dueDate).add(200, 'days')
    const min = moment(this.items.reduce((a, b)=>{return a.startDate < b.startDate ? a : b}).startDate).subtract(1, 'day')
    
    let secondaryTimeScale: unitOfTime.DurationConstructor
    let format: (date: moment.Moment) => string
    let secondaryFormat: (date: moment.Moment) => string
    switch (this.timeScale) {
      default:
      case 'day':
        this.columnWidth = 30
        this.minColumnWidth = 30
        // secondaryTimeScale = 'week'
        // format = (date: moment.Moment) => date.format('DD')
        // secondaryFormat = (startDate: moment.Moment) => startDate.format('MMM YYYY, Wo') + ' week'
        secondaryTimeScale = 'month'
        format = (date: moment.Moment) => date.format('DD')
        secondaryFormat = (startDate: moment.Moment) => startDate.format('MMM YYYY')
        break;
      case 'week':
        this.columnWidth = 60
        this.minColumnWidth = 60
        secondaryTimeScale = 'month'
        format = (date: moment.Moment) => date.format('Wo')
        secondaryFormat = (startDate: moment.Moment) => startDate.format('MMM YYYY')
        break;
      case 'month':
        this.columnWidth = 40
        this.minColumnWidth = 40
        secondaryTimeScale = 'quarter'
        format = (date: moment.Moment) => date.format('MMM')
        secondaryFormat = (date: moment.Moment) => 'Q'+date.format('Q YYYY')
        break;
      case 'quarter':
        this.columnWidth = 30
        this.minColumnWidth = 30
        secondaryTimeScale = 'year'
        format = (date: moment.Moment) => 'Q'+date.format('Q')
        secondaryFormat = (date: moment.Moment) => date.format('YYYY')
        break;
    }
    
    const duration = moment.duration(1, this.timeScale)
    let startDate = min.startOf(this.timeScale)
    this.chartStartDate = startDate.valueOf()
    while(startDate.isBefore(max)){
      if(this.timeScale != 'day')
        this.dates.push(format(startDate))
      else{
        if(helpers.includeDay(startDate.valueOf(), this.hideWeekend, this.holidays))
          this.dates.push(format(startDate))
      }
      startDate.add(duration)
    }

    // secondary date strip -- TODO: Can be initialized in previous while, optimization for later
    startDate = moment(this.chartStartDate.valueOf())
    const secondaryDuration = moment.duration(1, secondaryTimeScale)
    while(startDate.isBefore(max)){
      // const month = startDate.format('MMM')

      // const tmp = startDate.clone()
      // let counter = 0
      // const obj = {len: 0, value: secondaryFormat(startDate)}
      // startDate.add(secondaryDuration).startOf(secondaryTimeScale)
      // while(tmp.isBefore(startDate)){
      //   if(helpers.includeDay(tmp.valueOf(), this.hideWeekend, this.holidays))
      //     counter += 1
      //   tmp.add(duration)
      // }
      // obj.len = counter
      const tmp = startDate.clone()
      startDate.add(secondaryDuration).startOf(secondaryTimeScale)
      let len = startDate.diff(tmp, this.timeScale, true)
      if(this.timeScale == 'day'){
        while(tmp.isBefore(startDate)){
            if(!helpers.includeDay(tmp.valueOf(), this.hideWeekend, this.holidays))
              len -= 1
          tmp.add(duration)
        } 
      }
      this.secondaryDates.push({len: len, value: secondaryFormat(startDate)})

        // if(month != startDate.format('MMM'))
      //   this.secondaryDates[this.secondaryDates.length-1].value = startDate.format('MMM/')+month + this.secondaryDates[this.secondaryDates.length-1].value
      // else
      //   this.secondaryDates[this.secondaryDates.length-1].value = month + this.secondaryDates[this.secondaryDates.length-1].value
    }
  }

  insertCategories(){
    this.categories[0].startIdx = 0

    const newItem = new Item()
    newItem.title = this.categories[0].name
    newItem.category = this.categories[0].name
    newItem.index = this.categories[0].idx
    newItem.id = -1
    newItem.startDate = this.categories[0].min
    newItem.dueDate = this.categories[0].max
    newItem.color = 'grey'
    newItem.type = ItemType.category

    this.items.splice(0, 0, newItem)  // insert at index 0

    for(let i=2, j=1;i<this.items.length;i++){
      if(this.items[i].category != this.items[i-1].category){
        this.categories[j].startIdx = i

        const newItem = new Item()
        newItem.title = this.categories[j].name
        newItem.category = this.categories[j].name
        newItem.id = -1
        newItem.index = this.categories[j].idx
        newItem.startDate = this.categories[j].min
        newItem.dueDate = this.categories[j].max
        newItem.color = 'grey'
        newItem.type = ItemType.category
        this.items.splice(i, 0, newItem)
        j+=1
        i+=1
      }
    }
    this.updateIdMap()
  }
  getCategory(name: string): Category{
    const cat = this.categories.find(cat => cat.name == name)
    if(cat === undefined)
      throw "Category doesn't exist"
    return cat
  }
  removeCategories(){
    this.items = this.items.filter(a => a.type != ItemType.category)
  }
  updateIdMap(){
    for (let i = 0; i < this.items.length; i++) { // init after sorting by categories
      this.idMap[this.items[i].id] = i
    }
  }
  initCategories(){
    this.removeCategories()
    this.makeCategories()
    this.sortItemsByCategories()
  }
  makeCategories(){
    const categories = Object.entries(this.items.reduce<Record<string, {min: number, max: number, count: number}>>((prev, item) => {
      if(!prev[item.category]){
          prev[item.category] = {min: item.startDate, max: item.dueDate, count: 1}
          return prev
      }
      prev[item.category].count += 1
      if(prev[item.category].min > item.startDate)
          prev[item.category].min = item.startDate
      if(prev[item.category].max < item.dueDate)
          prev[item.category].max = item.dueDate
      return prev
    }, {}))
    categories.forEach(cat => {
      const c = this.getCategory(cat[0])
      c.min = cat[1].min
      c.max = cat[1].max
      c.count = cat[1].count
    });
  }
  sortItemsByCategories(){
    if(this.itemSort == ItemSort.custom){
      this.categories.sort((a, b) => a.idx - b.idx)
      this.items.sort( (a,b) =>
        // TODO: categories.find error -> object is possibly undefined
        this.getCategory(a.category).idx - this.getCategory(b.category).idx
        || a.indexInCategory - b.indexInCategory
      )
    } else if(this.itemSort == ItemSort.startDate){
      var order = this.categories.sort((a: any,b: any)=> a.min - b.min).map((e: any, i: any)=>e.name)
      this.items.sort((a,b)=> order.indexOf(a.category) - order.indexOf(b.category) || a.startDate - b.startDate)
    } else if(this.itemSort == ItemSort.endDate){
      const order = this.categories.sort((a: any,b: any)=> a.max - b.max).map((e: any, i: any)=>e.name)
      this.items.sort((a,b)=> order.indexOf(a.category) - order.indexOf(b.category) || a.dueDate - b.dueDate)
    }
  }
  sortItems(){
    if(this.itemSort == ItemSort.custom){
      this.items.sort( (a,b) => a.index - b.index)
    } else if(this.itemSort == ItemSort.startDate){
      this.items.sort( (a,b) => a.startDate - b.startDate)
    } else if(this.itemSort == ItemSort.endDate){
      this.items.sort( (a,b) => a.dueDate - b.dueDate)
    }
  }

  initItemDisplay(){
    this.items.forEach(item => {
      this.snapDate = this.timeScale == 'day'

      if(this.snapDate){
        item.left = moment(item.startDate).startOf(this.timeScale).diff(this.chartStartDate, this.timeScale) * this.columnWidth

        if(moment(item.dueDate).valueOf() == moment(item.dueDate).startOf(this.timeScale).valueOf()){
          item.width = moment(item.dueDate).diff(moment(item.startDate).startOf(this.timeScale), this.timeScale) * this.columnWidth
        } else
          item.width = (moment(item.dueDate).startOf(this.timeScale).diff(moment(item.startDate).startOf(this.timeScale), this.timeScale)+1) * this.columnWidth
      }
      else{
        item.left = moment(item.startDate).diff(this.chartStartDate, this.timeScale, true) * this.columnWidth
        item.width = moment(item.dueDate).diff(item.startDate, this.timeScale, true) * this.columnWidth
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
    this.startEdgeDrag(event)
    this.originalLeft = this.lastHovered.left
    return false  // event.preventDefault() // disable text select
  }
  startTaskRightEdgeDrag(event: any){
    this.dragging = DraggingType.taskEdgesRight
    this.startEdgeDrag(event)
    return false
  }
  startEdgeDrag(event: any){
    this.originalItem = this.lastHovered
    this.originalWidth = this.lastHovered.width
    this.draggedOriginal = {x: event.x, y: event.y}
    event.stopPropagation();
  }

  startTaskDrag(event: any){
    if(this.lastHovered.type == ItemType.category)
      return
    this.dragging = DraggingType.task
    this.originalItem = this.lastHovered
    this.originalLeft = this.lastHovered.left
    this.draggedOriginal = {x: event.x, y: event.y}
    // event.stopPropagation();
    return false
  }

  // originalIndex = 0
  verticalDragLinePos = 0
  startTaskVerticalDrag(event: any){
    this.originalItem = this.lastHovered
    // this.originalIndex = this.lastHovered.index
    this.draggedOriginal = {x: event.x, y: event.y}
    this.dragging = DraggingType.taskVertical
    this.onMouseMove(event)
    return false
  }

  @HostListener('mousemove', ['$event'])
  onMouseMove(event: MouseEvent | any){
    if(!this.originalItem)
      return false

    const _offset = {x: event.x - this.draggedOriginal.x, y: event.y - this.draggedOriginal.y}

    if(this.dragging == DraggingType.dependency && !this.clipLine){
      this.offset = {x: _offset.x - (this.chartRect.left - this.chartElem.nativeElement.scrollLeft), y: _offset.y - (this.chartRect.top - this.chartElem.nativeElement.scrollTop) - 40} // - header height
    }
    else if(this.dragging == DraggingType.taskEdgesLeft){
      this.originalItem.width = this.originalWidth - _offset.x
      this.originalItem.left = this.originalLeft + _offset.x
    }
    else if(this.dragging == DraggingType.taskEdgesRight){
      this.originalItem.width = this.originalWidth + _offset.x
    }
    else if(this.dragging == DraggingType.task){
      this.originalItem.left = this.originalLeft + _offset.x
    }
    else if(this.dragging == DraggingType.taskVertical){
      const idx = this.items.indexOf(this.originalItem)
      let newIdx = this.items.indexOf(this.lastHovered)
      if(this.groupByCategory){
        if(this.originalItem.type != ItemType.category){
          if(this.originalItem.category != this.lastHovered.category || this.lastHovered.type == ItemType.category)
            newIdx = this.clampToCategory(this.originalItem, this.lastHovered)
        }else{
          newIdx = this.clampCategory(this.originalItem, this.lastHovered)
        }
      }
      this.verticalDragLinePos = (newIdx + ((newIdx > idx) ? 1 : 0)) * this.taskHeight + 20 // 20 = header height in css - this.taskHeight
    }
    return false
  }

  @HostListener('mouseup', ['$event'])
  onMouseUp(event: MouseEvent | any) {
    if(!this.originalItem || (event.x == this.draggedOriginal.x && event.y == this.draggedOriginal.y)){
      this.dragging = DraggingType.none // just in case?
      return
    }
    if(this.dragging == DraggingType.dependency){
      if(this.clipLine
        && this.lastHovered.id != this.originalItem?.id // not the same
        && !this.lastHovered.dependant.includes(this.originalItem.id) // last doesnt already contain it
        && !this.originalItem.dependant.includes(this.lastHovered.id)){ // current isn't dependant on it
        this.originalItem.dependant.push(this.lastHovered.id)
        this.updateTaskDependency(this.originalItem)
      }
      this.dragging = DraggingType.none
    }
    if(this.originalItem && this.originalItem != this.lastHovered)  //TODO: State?
      this.originalItem.hover = false

    if((this.dragging == DraggingType.taskEdgesLeft || this.dragging == DraggingType.taskEdgesRight) && event.x != this.draggedOriginal.x){
      this.updateItemDates(this.originalItem)
      
      if(this.groupByCategory){
        this.initCategories()
        this.insertCategories()
      }
      this.initItemDisplay()
    }
    if(this.dragging == DraggingType.task && event.x != this.draggedOriginal.x){
      this.updateItemDates(this.originalItem)
      
      if(this.groupByCategory){
        this.initCategories()
        this.insertCategories()
      }
      this.initItemDisplay()
    }
    if(this.dragging == DraggingType.taskVertical && event.y != this.draggedOriginal.y){
      if(this.originalItem == this.lastHovered){
        this.dragging = DraggingType.none
        return false
      }

      if(this.groupByCategory){
        this.moveItemInArrayAndUpdateCategoryIndex(this.originalItem, this.lastHovered)
      } else{
        this.moveItemInArrayAndUpdateIndex(this.originalItem, this.lastHovered)
      }
      // this.updateTaskIndex(this.originalItem)
      // this.updateTaskIndex(this.items[newIndex])
      this.updateIdMap()
    }
    this.dragging = DraggingType.none
    return false  // event.preventDefault()
  }
  moveItemInArrayAndUpdateIndex(original: Item, newItem: Item){
    const newIndex = this.items.indexOf(newItem)
    const originalIndex = this.items.indexOf(original)
    let idx = this.items[originalIndex].index
    let last = idx
    let direction = 1
    
    if(newIndex < idx)
      direction = -1

    // range(from: original, to: newIndex, step: direction)
    for(let i = originalIndex+direction; i != newIndex+direction; i+=direction){
      last = this.items[i].index
      this.items[i].index = idx
      idx = last
      this.items[i-direction] = this.items[i]
    }
    this.items[newIndex] = original
    this.items[newIndex].index = last
  }
  moveItemInArrayAndUpdateCategoryIndex(original: Item, newItem: Item){
    if(original.type == ItemType.category){
      const newIndex = this.categories.findIndex(cat => cat.name == newItem.category)
      const originalIndex = this.categories.findIndex(cat => cat.name == original.category)
      let idx = this.categories[originalIndex].idx
      let last = idx
      let direction = 1
      
      const originalCat = this.getCategory(original.category)
      const newCat = this.getCategory(newItem.category)

      const currentCat = this.items.slice(originalCat.startIdx, originalCat.startIdx + originalCat.count+1)
      if(newIndex < originalIndex){
        direction = -1
        const other = this.items.slice(newCat.startIdx, originalCat.startIdx)
        this.items.splice(newCat.startIdx, originalCat.startIdx + originalCat.count+1, ...currentCat, ...other)
      } else{
        const other = this.items.slice(originalCat.startIdx + originalCat.count+1, newCat.startIdx + newCat.count+1)  
        this.items.splice(originalCat.startIdx, newCat.startIdx + newCat.count+1, ...other, ...currentCat)
      }

      // range(from: originalIndex, to: newIndex, step: direction)
      let i = originalIndex+direction
      for(; i != newIndex+direction; i+=direction){
        last = this.categories[i].idx
        this.categories[i].idx = idx
        idx = last
        this.categories[i].startIdx -= direction*(originalCat.count + 1)
        this.categories[i-direction] = this.categories[i]
      }
      this.categories[newIndex] = originalCat
      this.categories[newIndex].idx = last
      if(direction == 1)
        this.categories[newIndex].startIdx = this.categories[i-2*direction].startIdx + this.categories[i-2*direction].count + 1
      else
        this.categories[newIndex].startIdx = this.categories[i-2*direction].startIdx - this.categories[newIndex].count - 1
    }
    else{
      let originalIdx = this.items.indexOf(original)
      let newItemIndex = this.items.indexOf(newItem)
      newItemIndex = this.clampToCategory(original, newItem)
      if(newItemIndex > originalIdx){
        for(let i = originalIdx+1; i <= newItemIndex; i++){
          this.items[i].indexInCategory = this.items[i-1].indexInCategory
          this.items[i-1] = this.items[i]
        }
        this.items[newItemIndex] = original
        this.items[newItemIndex].indexInCategory = newItem.indexInCategory
      } else{
        for(let i = originalIdx-1; i >= newItemIndex; i--){
          this.items[i].indexInCategory = this.items[i+1].indexInCategory
          this.items[i+1] = this.items[i]
        }
        this.items[newItemIndex] = original
        this.items[newItemIndex].indexInCategory = newItem.indexInCategory
      }
    }
  }

  @HostListener('mouseleave')
  onMouseLeave() {
    this.dragging = DraggingType.none
    this.lastHovered.hover = false
    if(this.originalItem)
      this.originalItem.hover = false
    return false  // event.preventDefault()
  }

  @HostListener('wheel', ['$event'])
  onScroll(event: WheelEvent){
    let incAmount = Math.floor(this.columnWidth / 10)

    if(!event.ctrlKey)
      return
    if(event.deltaY < 0){      
      if(this.columnWidth - incAmount < this.minColumnWidth)
        return false
      incAmount = -incAmount
    }

    this.columnWidth += incAmount
    this.initItemDisplay()
    // event.stopPropagation()
    return false
  }

  clampToCategory(original: Item, newItem: Item): number{
    if(original.category != newItem.category || newItem.type == ItemType.category){
      const cat = this.getCategory(original.category)
      if(this.items.indexOf(newItem) < this.items.indexOf(original))
        return cat.startIdx + 1
      else
        return cat.startIdx + cat.count
    }

    return this.items.indexOf(newItem)
  }
  clampCategory(original: Item, newItem: Item): number{
    const originalCat = this.getCategory(original.category)
    if(original.category == newItem.category)
      return originalCat.startIdx
    const newCat = this.getCategory(newItem.category)
    if(this.items.indexOf(newItem) < this.items.indexOf(original))
      return newCat.startIdx
    else
      return newCat.startIdx + newCat.count
  }

  updateItemDates(item: Item){
    //TODO: weekend bug?
    // problems snapping because start / due date aren't rounded to day

    var bias = (item.left % this.columnWidth > this.columnWidth / 2 && this.snapDate) ? 1 : 0
    item.startDate = this.chartStartDate + (item.left / this.columnWidth + bias) * moment.duration(1, this.timeScale).asMilliseconds()
    // item.startDate = moment(this.chartStartDate).add(moment.duration((item.left / this.columnWidth + bias), this.timeScale)).valueOf()
    const rem = (item.left + item.width) % this.columnWidth
    bias = (rem < this.columnWidth / 2 && rem != 0 && this.snapDate) ? -1 : 0
    item.dueDate = this.chartStartDate + ((item.width + item.left) / this.columnWidth + bias) * moment.duration(1, this.timeScale).asMilliseconds()
    // item.dueDate = moment(this.chartStartDate).add(moment.duration(((item.width + item.left) / this.columnWidth + bias), this.timeScale)).valueOf()
    
    // if(this.snapDate){
    //   item.startDate = moment(this.chartStartDate + (item.left / this.columnWidth) * moment.duration(1, 'day').asMilliseconds()).startOf('day').valueOf()
    //   if(item.dueDate != moment(item.dueDate).endOf('day').valueOf())
    //   item.dueDate = moment(this.chartStartDate + ((item.left + item.width) / this.columnWidth) * moment.duration(1, 'day').asMilliseconds()).endOf('day').valueOf()
    // }
    this.updateTaskDates(item)
    this.updateDependencies(item)
  }
  updateDependencies(item: Item, limit = 50){
    //TODO: dragging left egde offsets deps by 1 to the right???

    const updateDependency = (prev: Item, next: Item, limit: number) => {
      if(limit == 0) return
      next.dueDate += prev.dueDate - next.startDate + TimeScale.day
      next.startDate = prev.dueDate + TimeScale.day
      // if(next.id == 3){
      //   console.log(new Date(prev.dueDate))
      //   console.log(new Date(next.startDate))
      // }
      // if(moment(next.startDate).valueOf() == moment(next.startDate).startOf('day').valueOf())
      //   next.startDate -= TimeScale.day
      this.updateTaskDates(next)
      next.dependant.forEach(next2 => updateDependency(next, this.items[this.idMap[next2]], limit - 1))
    }
    item.dependant.forEach(_item => updateDependency(item, this.items[this.idMap[_item]], limit - 1))
  }

  dependencyPopUp(item: Item, event: any){
    alert("In development: *dependency popup here*")
    event.stopPropagation()
    return false
  }
  taskDoubleClick(item: Item){
    this.router.navigate(['/project/' + item.projectId + '/task/' + item.id])
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
      itemSort: this.itemSort,
      hideWeekend: this.hideWeekend,
      groupByCategory: this.groupByCategory,
      holidays: this.holidays,
      columns: this.columns
    } })

    ref.beforeClosed().subscribe((data: any)=>{
      if(data){
        this.columns = data.columns
        if(this.itemSort != data.itemSort){
          this.itemSort = data.itemSort
          if(this.groupByCategory){
            this.initCategories() // can be more efficient by only sorting and updating idMap again
            this.insertCategories()
          }
          else{
            this.sortItems()
            this.updateIdMap()
          }
          this.initItemDisplay() // update category item position and width
        }
        if(this.groupByCategory != data.groupByCategory){
          this.groupByCategory = data.groupByCategory
          if(this.groupByCategory){
            this.initCategories()
            this.insertCategories()
            this.initItemDisplay() // update category item position and width
        }else{
            this.removeCategories()
            if(this.itemSort == ItemSort.custom)
              this.sortItems()
            this.updateIdMap()
          }
          this.initItemDisplay()
        }
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

  infoColumnDragDrop(event: CdkDragDrop<string[]>){
    moveItemInArray(this.columns, event.previousIndex, event.currentIndex)
  }

  // TODO: check if its a milestone
  updateTaskDates(item: Item){
    // this.taskService.updateTask({id: item.id, startDate: new Date(item.startDate), dueDate: new Date(item.dueDate)})
  }
  updateTaskIndexes(item: Item){
    // this.taskService.updateTask({id: item.id, index: item.index})
  }
  updateTaskDependency(item: Item){
    // this.taskService.updateTask({id: item.id, dependentTasks: item.dependant})
  }
}
