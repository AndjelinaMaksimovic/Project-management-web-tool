import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-member-item',
  standalone: true,
  imports: [],
  templateUrl: './member-item.component.html',
  styleUrl: './member-item.component.css'
})
export class MemberItemComponent {
  @Input() name: string = "";
  @Input() desc: string = "";
  @Input() image: string = "";
}
