import { NgIf } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-progressbar',
  standalone: true,
  imports: [ NgIf ],
  templateUrl: './progressbar.component.html',
  styleUrl: './progressbar.component.css'
})
export class ProgressbarComponent {
  @Input() progress: number = 0;
  @Input() color: string = "black";
}
