import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-topnav',
  standalone: true,
  imports: [ MatButtonModule, MatIconModule, MatInputModule ],
  templateUrl: './topnav.component.html',
  styleUrl: './topnav.component.css'
})

export class TopnavComponent {

}
