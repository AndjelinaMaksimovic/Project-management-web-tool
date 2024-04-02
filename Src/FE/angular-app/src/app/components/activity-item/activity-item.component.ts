import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-activity-item',
  standalone: true,
  imports: [],
  templateUrl: './activity-item.component.html',
  styleUrl: './activity-item.component.scss'
})
export class ActivityItemComponent {
  @Input() name: String = "";
  @Input() jobPosition: String = "";
  @Input() activity: String = "";
  @Input() date: String = "";
}
