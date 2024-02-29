import { Routes } from '@angular/router';
import { TestrouteComponent } from './pages/testroute/testroute.component';
import { DetailsComponent } from './pages/details/details.component';

export const routes: Routes = [
    {path: "posts", component: TestrouteComponent },
    {path: "post/:id", component: DetailsComponent },
];
