import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { RegisterComponent } from './pages/register/register.component';
import { ErrorComponent } from './pages/error/error.component';
import { ActivateComponent } from './pages/activate/activate.component';
// import { NewProjectComponent } from './pages/new-project/new-project.component';
import { GanttTestComponent } from './pages/gantt-test/gantt-test.component';
import { MyTasksComponent } from './pages/my-tasks/my-tasks.component';
import { CanSeeCompanyMembers, LoggedIn, NotSuperUser } from './services/auth.service';
import { NotLoggedIn } from './services/auth.service';
import { ProjectDetailsComponent } from './pages/project-details/project-details.component';
import { ProfileComponent } from './pages/profile/profile.component';
import { NewTaskComponent } from './pages/new-task/new-task.component';
import { TaskComponent } from './pages/task/task.component';
import { ArchivedProjectsComponent } from './pages/archived-projects/archived-projects.component';
import { MembersComponent } from './pages/members/members.component';
import { ChangePasswordComponent } from './pages/change-password/change-password.component';

export const routes: Routes = [
  {
    path: '',
    canActivate: [LoggedIn],
    children: [
      { path: 'profile/:userId', title: 'Profile | Codedberries', component: ProfileComponent },
      { path: 'company-members', title: 'Company members | Codedberries', canActivate: [CanSeeCompanyMembers],component: MembersComponent},
      {
        path: '',
        canActivate: [NotSuperUser],
        // component: HomeComponent,
        children: [
          {
            path: '',
            canActivate: [NotSuperUser],
            component: HomeComponent,
          },
          // { path: 'new-project', component: NewProjectComponent },
          { path: 'project/:id/tasks', title: 'Project tasks | Codedberries', component: MyTasksComponent },
          { path: 'project/:id/new-task', title: 'New task | Codedberries', component: NewTaskComponent },
          { path: 'project/:id/details', title: 'Project details | Codedberries', component: ProjectDetailsComponent },
          { path: 'project/:id/task/:taskId', title: 'Task | Codedberries', component: TaskComponent },
          { path: 'project/:id/members', title: 'Members | Codedberries', component: MembersComponent, data: { isProject: true} },
          { path: 'archived-projects', title: 'Archived projects | Codedberries', component: ArchivedProjectsComponent },
        ]
      },
    ]
  },
  { path: 'login', title: 'Log in | Codedberries', canActivate: [NotLoggedIn], component: LoginComponent },
  { path: 'register', title: 'Register | Codedberries', component: RegisterComponent },
  { path: 'activate', title: 'Activate | Codedberries', component: ActivateComponent },
  { path: 'changePassword', title: 'Change password | Codedberries', component: ChangePasswordComponent },
  { path: '**', title: 'Codedberries | Page not found', component: ErrorComponent },
];