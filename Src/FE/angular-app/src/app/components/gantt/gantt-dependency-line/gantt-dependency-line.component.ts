import { NgClass, NgIf } from '@angular/common';
import { Component, HostListener, Input } from '@angular/core';

@Component({
  selector: 'app-gantt-dependency-line',
  standalone: true,
  imports: [ NgIf, NgClass ],
  templateUrl: './gantt-dependency-line.component.html',
  styleUrl: './gantt-dependency-line.component.css'
})
export class GanttDependencyLineComponent {
  @Input() from: Coordinates = new Coordinates(0, 0)
  @Input() offset: Coordinates = new Coordinates(0, 0)
  @Input() taskHeight: number = 20
  @Input() barHeight: number = 14
  @Input() hover: boolean = false

  firstOffset = 5 // line from the origin task to the right, and goal task from the left

  abs(x: number): number{ return (x >= 0) ? x : -x }

  @HostListener('mouseenter', ['$event']) //TODO: not working
  onHoverEnter(e: Event){
    e.stopImmediatePropagation()
    e.stopPropagation()
    return false
  }
}
class Coordinates{constructor(public x: number, public y: number){}}