import { Component } from '@angular/core';
import { TopnavComponent } from '../../components/topnav/topnav.component';

@Component({
  selector: 'app-error',
  standalone: true,
  imports: [ TopnavComponent ],
  templateUrl: './error.component.html',
  styleUrl: './error.component.css'
})
export class ErrorComponent {

}
