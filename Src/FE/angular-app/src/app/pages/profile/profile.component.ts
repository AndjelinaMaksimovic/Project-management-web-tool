import { Component } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { MaterialModule } from '../../material/material.module';
import { MatDividerModule } from '@angular/material/divider';
import { User, UserService } from '../../services/user.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [NavbarComponent, MaterialModule, MatDividerModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent {
  userId: number = 0;
  user: User | undefined;

  constructor(private userService: UserService, private route: ActivatedRoute) {}
  async ngOnInit() {
    this.route.params.subscribe((params) => {
      this.userId = parseInt(params['userId']);
    });
    this.user = await this.userService.getUser(this.userId);
  }
}
