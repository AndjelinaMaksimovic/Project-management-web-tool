import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { RegisterComponent } from './pages/register/register.component';
import { ErrorComponent } from './pages/error/error.component';
import { ActivateComponent } from './pages/activate/activate.component';
import { NewProjectComponent } from './pages/new-project/new-project.component';
import { MyTasksComponent } from './pages/my-tasks/my-tasks.component';
import { LoggedIn } from './services/auth.service';
import { ProjectDetailsComponent } from './pages/project-details/project-details.component';

export const routes: Routes = [
  { path: '', component: HomeComponent, canActivate: [LoggedIn] },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'activate', component: ActivateComponent },
  { path: 'new-project', component: NewProjectComponent, canActivate: [LoggedIn] },
  { path: 'my-tasks', component: MyTasksComponent, canActivate: [LoggedIn] },
  { path: 'project-details', component: ProjectDetailsComponent },
  { path: '**', component: ErrorComponent },
];
