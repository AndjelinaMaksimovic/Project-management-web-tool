import { Component } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { MaterialModule } from '../../material/material.module';
import { MatDividerModule } from '@angular/material/divider';
import { User, UserService } from '../../services/user.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [NavbarComponent, MaterialModule, MatDividerModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent {
  user: User | undefined;

  constructor(private userService: UserService) {}
  async ngOnInit() {
    await this.userService.fetchUsers();
    this.user = this.userService.getUsers()[0];
    console.log("this.user",this.user)
  }
}
