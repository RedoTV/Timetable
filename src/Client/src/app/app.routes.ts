import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LogInComponent } from './log-in/log-in.component';
import { SignUpComponent } from './sign-up/sign-up.component';

export const routes: Routes = [
    {
        path: "",
        pathMatch: "full",
        component: HomeComponent,
        data: {
            animation:"slideInAnimation"
        }
    },
    {
        path:"login",
        component: LogInComponent,
        data: {
            animation:"slideInAnimation"
        }
    },
    {
        path:"signup",
        component:SignUpComponent,
        data: {
            animation:"slideInAnimation"
        }
    }
];
