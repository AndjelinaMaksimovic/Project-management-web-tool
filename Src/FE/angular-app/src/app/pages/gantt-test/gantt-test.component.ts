import { Component } from '@angular/core';
import { GanttComponent } from '../../components/gantt/gantt.component';

@Component({
  selector: 'app-gantt-test',
  standalone: true,
  imports: [ GanttComponent ],
  templateUrl: './gantt-test.component.html',
  styleUrl: './gantt-test.component.css'
})
export class GanttTestComponent {

}
