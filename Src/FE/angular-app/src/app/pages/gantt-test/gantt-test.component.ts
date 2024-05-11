import { Component, OnInit } from '@angular/core';
import { GanttComponent } from '../../components/gantt/gantt.component';
import { Item, GanttColumn, TimeScale, ItemType, Column } from '../../components/gantt/item';

@Component({
  selector: 'app-gantt-test',
  standalone: true,
  imports: [ GanttComponent ],
  templateUrl: './gantt-test.component.html',
  styleUrl: './gantt-test.component.css'
})
export class GanttTestComponent{
  items = [
    new Item(1, 5, 5, 1, "Item 1 overflow overflow overflow overflow", "desc", "category 1", "Low", "status 1", Date.now() - TimeScale.day, Date.now(), [{firstName: 'Milan', lastName: 'Milanovic', id: 1, profilePicture: ''}], [2], 100, ItemType.task, '#5096A4'),
    new Item(2, 1, 1, 1, "Item 2", "desc 2", "category 1", "Low", "status 3", Date.now() + TimeScale.day*2, Date.now() + TimeScale.day * 3, [{firstName: 'Milan', lastName: 'Milanovic', id: 1, profilePicture: ''}, {firstName: 'Ivan', lastName: 'Ivanovic', id: 2, profilePicture: ''}], [3], 100, ItemType.task, '#1ab99c'),
    new Item(3, 2, 2, 1, "Item 3", "desc 3", "category 2", "High", "status 3", Date.now() + TimeScale.day*4, Date.now() + TimeScale.day * 5, [], [], 20, ItemType.task, '#1ab99c'),
    new Item(4, 4, 3, 1, "Milestone", "desc 4", "category 2", "Low", "status 1", Date.now(), Date.now(), [{firstName: 'Ivan', lastName: 'Ivanovic', id: 1, profilePicture: ''}], [], 50, ItemType.milestone, '#c24e4e'),
    new Item(5, 3, 4, 1, "Item 4", "desc 4", "category 3", "Low", "status 1", Date.now(), Date.now() + TimeScale.day * 10, [{firstName: 'Milan', lastName: 'Milanovic', id: 1, profilePicture: ''}], [], 50, ItemType.task, '#c24e4e'),
  ]
  // items: Item[] = []
  // constructor(){
  //   const limit = 300
  //   for (let i = 0; i < limit; i++) {
  //     const item = new Item(i, 1, "Item "+i, "desc", "category 1", "Low", "status 1", Date.now() + TimeScale.day*(i+3), Date.now()+TimeScale.day*(i+3), [{firstName: 'Milan', lastName: 'Milanovic', id: 1, profilePicture: ''}], (i==limit-1) ? [] : [i+1], 100, ItemType.task, '#5096A4')
  //     this.items.push(item)
  //   }
  // }

  columns: Column[] = [
    {type: GanttColumn.tasks, width: 130},
    {type: GanttColumn.users, width: 130},
    {type: GanttColumn.progress, width: 80}]
  TimeScale = TimeScale
}
