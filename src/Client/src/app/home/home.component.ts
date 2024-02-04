import { Component } from '@angular/core';
import { LogInComponent } from "../log-in/log-in.component";

@Component({
    selector: 'app-home',
    standalone: true,
    templateUrl: './home.component.html',
    styleUrl: './home.component.css',
    imports: [LogInComponent]
})
export class HomeComponent {

}
