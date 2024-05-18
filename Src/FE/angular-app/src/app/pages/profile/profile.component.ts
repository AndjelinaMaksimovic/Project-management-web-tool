import { Component } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { MaterialModule } from '../../material/material.module';
import { MatDividerModule } from '@angular/material/divider';
import { User, UserService } from '../../services/user.service';
import { ActivatedRoute } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { firstValueFrom } from 'rxjs';
import { EditableNameComponent } from './editable-name/editable-name.component';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
// import { environment } from '../../environments/environment';


@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [NavbarComponent, MaterialModule, MatDividerModule, EditableNameComponent, CommonModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent {
  userId: string = "me";
  loggedInUser: number | undefined;
  user: any;
  
  timestamp: number = Date.now();
  getProfileImagePath(){
    return `${environment.apiUrl}/User/users/avatars/${this.user.id}?timestamp=${this.timestamp}`
  }
  private httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    withCredentials: true,
    observe: 'response' as 'response',
  };
  constructor(
    private userService: UserService,
    private route: ActivatedRoute,
    private authService: AuthService,
    private http: HttpClient
  ) {}
  async ngOnInit() {
    this.route.params.subscribe((params) => {
      this.userId = params['userId'];
    });
    this.loggedInUser = await this.authService.getMyId();
    this.user = this.userId === "me" ? await this.userService.getMe() : await this.userService.getUser(parseInt(this.userId));
  }

  async sendDataToServer(data: {userId: string, imageBytes: string, imageName: string}){
    console.log("sending data", data);
    await firstValueFrom(this.http.post<any>(
      environment.apiUrl + `/User/setProfilePicture`,
      {
        ...data
      },
      {
        ...this.httpOptions,
      }
    ));
  }
  async uploadImage(base64String: string, imageName: string) {
    const userId = this.user.id; // Replace with actual user ID
    const data = {
      userId: userId,
      imageBytes: base64String,
      imageName: imageName
    };
    try{
      await this.sendDataToServer(data);
    } catch(e: unknown){
      console.log(e);
    }
    this.timestamp = Date.now();
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onloadend = () => {
      const base64String = reader.result?.toString().split(',')[1];
      if(!base64String) return;
      console.log("base64String", base64String);
      // Now you have the base64 string of the image
      this.uploadImage(base64String, "./test-image-02.jpg");
    };
  }
}
