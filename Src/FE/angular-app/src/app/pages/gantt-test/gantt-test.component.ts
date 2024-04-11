import { Component, OnInit } from '@angular/core';
import { GanttComponent } from '../../components/gantt/gantt.component';
import { Item, GanttColumn, TimeScale, Milestone } from '../../components/gantt/item';

@Component({
  selector: 'app-gantt-test',
  standalone: true,
  imports: [ GanttComponent ],
  templateUrl: './gantt-test.component.html',
  styleUrl: './gantt-test.component.css'
})
export class GanttTestComponent{
  items = [
    new Item(1, "Item 1 overflow overflow overflow overflow", "desc", "category 1", "Low", "status 1", Date.now() - TimeScale.day, Date.now(), [{firstName: 'Milan', lastName: 'Milanovic', id: 1, profilePicture: ''}], [2], '#5096A4'),
    new Item(2, "Item 2", "desc 2", "category 1", "Low", "status 3", Date.now() + TimeScale.day*2, Date.now() + TimeScale.day * 3, [{firstName: 'Milan', lastName: 'Milanovic', id: 1, profilePicture: ''}, {firstName: 'Ivan', lastName: 'Ivanovic', id: 2, profilePicture: ''}], [3], '#1ab99c'),
    new Item(3, "Item 3", "desc 3", "category 2", "High", "status 3", Date.now() + TimeScale.day*4, Date.now() + TimeScale.day * 5, [], [], '#1ab99c'),
    new Item(4, "Item 4", "desc 4", "category 1", "Low", "status 1", Date.now(), Date.now() + TimeScale.day * 10, [{firstName: 'Milan', lastName: 'Milanovic', id: 1, profilePicture: ''}], [], '#c24e4e'),
  ]
  milestones = [
    new Milestone(1, "Milestone 1", Date.now() + TimeScale.day)
  ]
  columns = [GanttColumn.tasks, GanttColumn.users]
  // columns = [GanttColumn.tasks]
  colWidths = [100, 250]
  TimeScale = TimeScale
}
