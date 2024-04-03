import { Component, Input } from '@angular/core';
import { NgStyle } from '@angular/common';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-status-item',
  standalone: true,
  imports: [ NgStyle, NgClass ],
  templateUrl: './status-item.component.html',
  styleUrl: './status-item.component.css'
})
export class StatusItemComponent {
  @Input() title: String = "";
  @Input() desc: String = "";
  @Input() faicon: String = "";
  @Input() color: String = "";
  @Input() bold: boolean = false;
}
