import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { RegisterComponent } from './pages/register/register.component';
import { ErrorComponent } from './pages/error/error.component';
import { ActivateComponent } from './pages/activate/activate.component';
import { GanttTestComponent } from './pages/gantt-test/gantt-test.component';
import { MyTasksComponent } from './pages/my-tasks/my-tasks.component';
import { LoggedIn } from './services/auth.service';
import { NotLoggedIn } from './services/auth.service';
import { ProjectDetailsComponent } from './pages/project-details/project-details.component';
import { TaskComponent } from './pages/task/task.component';

export const routes: Routes = [
  { path: '', title: 'Codedberries | Home', component: HomeComponent, canActivate: [LoggedIn] },
  { path: 'login', title: 'Codedberries | Log in', component: LoginComponent, canActivate: [NotLoggedIn] },
  { path: 'register', title: 'Codedberries | Register', component: RegisterComponent },
  { path: 'activate', title: 'Codedberries | Activate', component: ActivateComponent },
  { path: 'project/:id/tasks', title: 'Codedberries | Project tasks', component: MyTasksComponent, canActivate: [LoggedIn] },
  { path: 'project/:id/details', title: 'Codedberries | Project details', component: ProjectDetailsComponent, canActivate: [LoggedIn] },
  { path: 'project/:projectId/task/:taskId', title: 'Codedberries | Task', component: TaskComponent, canActivate: [LoggedIn] },
  { path: 'gantt-test', component: GanttTestComponent },
  { path: '**', title: 'Codedberries | Page not found', component: ErrorComponent },
  { path: '**', title: 'Codedberries | Page not found', component: ErrorComponent },
];
