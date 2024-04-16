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
  @Input() hover: boolean = false

  abs(x: number): number{ return (x >= 0) ? x : -x }

  @HostListener('mouseenter', ['$event'])
  onHoverEnter(e: Event){
    e.stopImmediatePropagation()
    e.stopPropagation()
    return false
  }
}
class Coordinates{constructor(public x: number, public y: number){}}