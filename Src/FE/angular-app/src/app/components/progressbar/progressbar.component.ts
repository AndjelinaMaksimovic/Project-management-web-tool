import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-progressbar',
  standalone: true,
  imports: [],
  templateUrl: './progressbar.component.html',
  styleUrl: './progressbar.component.css'
})
export class ProgressbarComponent {
  @Input() progress: Number = 0;
  @Input() color: string = "black";
}