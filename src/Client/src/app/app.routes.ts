import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LogInComponent } from './log-in/log-in.component';

export const routes: Routes = [
    {
        path: "",
        pathMatch: "full",
        component: HomeComponent
    },
    {
        path:"login",
        component: LogInComponent
    }
];
