import { Component } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { MaterialModule } from '../../material/material.module';
import {MatDividerModule} from '@angular/material/divider';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [ NavbarComponent, MaterialModule, MatDividerModule ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent {

}