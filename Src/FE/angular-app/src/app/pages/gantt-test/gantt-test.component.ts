import { Component, OnInit } from '@angular/core';
import { GanttComponent } from '../../components/gantt/gantt.component';
import { Item, GanttColumn, TimeScale } from '../../components/gantt/item';

@Component({
  selector: 'app-gantt-test',
  standalone: true,
  imports: [ GanttComponent ],
  templateUrl: './gantt-test.component.html',
  styleUrl: './gantt-test.component.css'
})
export class GanttTestComponent implements OnInit{
  items = [
    new Item("Item 1", Date.now() - TimeScale.day, Date.now()),
    new Item("Item 2", Date.now(), Date.now() + TimeScale.day)
  ]
  columns = [GanttColumn.title, GanttColumn.users]

  async ngOnInit() {
    // setTimeout(()=>{this.items.splice(1, 0, {...this.items[0]}); this.items[0].title = "Old item"}, 3_000)
  }
}
