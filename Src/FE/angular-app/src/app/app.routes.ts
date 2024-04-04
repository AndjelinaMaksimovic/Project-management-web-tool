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

export const routes: Routes = [
  { path: '', component: HomeComponent, canActivate: [LoggedIn] },
  { path: 'login', component: LoginComponent, canActivate: [NotLoggedIn] },
  { path: 'register', component: RegisterComponent },
  { path: 'activate', component: ActivateComponent },
  { path: 'project/:id/tasks', component: MyTasksComponent, canActivate: [LoggedIn] },
  { path: 'project/:id/details', component: ProjectDetailsComponent, canActivate: [LoggedIn] },
  { path: 'gantt-test', component: GanttTestComponent },
  { path: '**', component: ErrorComponent },
];
