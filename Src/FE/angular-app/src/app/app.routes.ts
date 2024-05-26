import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { RegisterComponent } from './pages/register/register.component';
import { ErrorComponent } from './pages/error/error.component';
import { ActivateComponent } from './pages/activate/activate.component';
// import { NewProjectComponent } from './pages/new-project/new-project.component';
import { GanttTestComponent } from './pages/gantt-test/gantt-test.component';
import { MyTasksComponent } from './pages/my-tasks/my-tasks.component';
import { LoggedIn, NotSuperUser } from './services/auth.service';
import { NotLoggedIn } from './services/auth.service';
import { ProjectDetailsComponent } from './pages/project-details/project-details.component';
import { ProfileComponent } from './pages/profile/profile.component';
import { NewTaskComponent } from './pages/new-task/new-task.component';
import { TaskComponent } from './pages/task/task.component';
import { ArchivedProjectsComponent } from './pages/archived-projects/archived-projects.component';
import { MembersComponent } from './pages/members/members.component';

export const routes: Routes = [
  {
    path: '',
    canActivate: [LoggedIn],
    children: [
      { path: '', canActivate: [NotSuperUser], component: HomeComponent },
      { path: 'company-members', title: 'Company members | Codedberries', component: MembersComponent},
      // { path: 'new-project', component: NewProjectComponent },
      { path: 'project/:id/tasks', title: 'Project tasks | Codedberries', component: MyTasksComponent },
      { path: 'project/:id/new-task', title: 'New task | Codedberries', component: NewTaskComponent },
      { path: 'project/:id/details', title: 'Project details | Codedberries', component: ProjectDetailsComponent },
      { path: 'profile/:userId', title: 'Profile | Codedberries', component: ProfileComponent },
      { path: 'project/:id/task/:taskId', title: 'Task | Codedberries', component: TaskComponent },
      { path: 'members/:id', title: 'Members | Codedberries', component: MembersComponent, data: { isProject: true} },
      { path: 'archived-projects', title: 'Archived projects | Codedberries', component: ArchivedProjectsComponent },
      { path: 'gantt-test', component: GanttTestComponent },
    ]
  },
  { path: 'login', title: 'Log in | Codedberries', canActivate: [NotLoggedIn], component: LoginComponent },
  { path: 'register', title: 'Register | Codedberries', component: RegisterComponent },
  { path: 'activate', title: 'Activate | Codedberries', component: ActivateComponent },
  { path: '**', title: 'Codedberries | Page not found', component: ErrorComponent },
];
/*
signalr, socket za notifikacije
paginacija aktivnosti
*/