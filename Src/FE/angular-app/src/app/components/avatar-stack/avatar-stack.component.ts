import { Component, Input } from '@angular/core';
import { AvatarService } from '../../services/avatar.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-avatar-stack',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './avatar-stack.component.html',
  styleUrl: './avatar-stack.component.css',
})
export class AvatarStackComponent {
  @Input() userIds: (string | number)[] = [];
  get avatarUrls(){
    return this.userIds.map(id => this.avatarService.getProfileImagePath(id));
  }
  constructor(public avatarService: AvatarService) {}
}
