import { Component } from '@angular/core';
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
import { TopnavComponent } from '../../components/topnav/topnav.component';
import { ProjectService } from '../../services/project.service';
import { ActivityItemComponent } from '../../components/activity-item/activity-item.component';
import { TaskService } from '../../services/task.service';
import { TaskCardComponent } from '../../components/task-card/task-card.component';
import { ProjectItemComponent } from '../../components/project-item/project-item.component';
import { AvatarService } from '../../services/avatar.service';
import { MarkdownModule, provideMarkdown } from 'ngx-markdown';
import { PageEvent } from '@angular/material/paginator';
// import { environment } from '../../environments/environment';


@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [TopnavComponent, MaterialModule, MatDividerModule, EditableNameComponent, CommonModule, ActivityItemComponent, TaskCardComponent, ProjectItemComponent, MarkdownModule],
  providers: [provideMarkdown()],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent {
  userId: string = "me";
  loggedInUser: number | undefined;
  user: any;

  allTasksAccordionVisible: boolean = false;
  allProjectsAccordionVisible: boolean = false;

  get tasks() {
    return this.taskService.getTasks();
  }

  get projects(){
    return this.projectService.getProjects().filter(project => !project.archived);
  }
  
  activities: any[] = [];
  
  paginatorLen = 0
  paginatorPageSize = 5
  viewActivities: any[] = []
  
  timestamp: number = Date.now();
  getProfileImagePath(){
    return this.avatarService.getProfileImagePath(this.user?.id);
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
    private http: HttpClient,
    private projectService: ProjectService,
    private avatarService: AvatarService,
    private taskService: TaskService
  ) {}
  async ngOnInit() {
    this.route.params.subscribe((params) => {
      this.userId = params['userId'];
    });
    this.loggedInUser = await this.authService.getMyId();
    this.user = this.userId === "me" ? await this.userService.getMe() : await this.userService.getUser(parseInt(this.userId));
    this.activities = await this.projectService.allUserActivities()

    await this.projectService.fetchUserProjects(this.loggedInUser!);
    await this.taskService.fetchUserTasks({ assignedTo: this.loggedInUser! });

    if(this.tasks.length != 0) this.allTasksAccordionVisible = true;
    if(this.projects.length != 0) this.allProjectsAccordionVisible = true;
    this.activities = this.activities.sort((a: any, b: any) => a.time > b.time ? -1 : 1)
    this.paginatorLen = this.activities.length
    this.viewActivities = this.activities.slice(0, this.paginatorPageSize)
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

  toggleTasks() {
    this.allTasksAccordionVisible = !this.allTasksAccordionVisible;
  }
  
  toggleProjects() {
    this.allProjectsAccordionVisible = !this.allProjectsAccordionVisible;
  }
  
  pageChange(e: PageEvent){
    const offset = e.pageIndex * e.pageSize
    this.viewActivities = this.activities.slice(offset, offset + e.pageSize)
  }
}
