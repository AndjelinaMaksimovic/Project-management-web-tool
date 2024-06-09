import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { AvatarService } from '../../services/avatar.service';
import { CommonModule } from '@angular/common';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-avatar-stack',
  standalone: true,
  imports: [CommonModule, MatTooltipModule],
  templateUrl: './avatar-stack.component.html',
  styleUrl: './avatar-stack.component.css',
})
export class AvatarStackComponent implements OnChanges {
  @Input() users: any[] = [];
  @Input() size: number = 32;
  tooltip: string = ''
  get avatarUrls(){
    return this.users.map(user => this.avatarService.getProfileImagePath(user.id));
  }
  constructor(public avatarService: AvatarService) {}
  ngOnChanges(changes: SimpleChanges): void {
    if(changes['users'])
      this.tooltip = this.users.map(u => u.firstName + ' ' + u.lastName).join('\n')
  }
}
