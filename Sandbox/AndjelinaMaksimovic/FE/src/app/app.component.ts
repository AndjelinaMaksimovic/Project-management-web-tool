import { Component, Input } from '@angular/core';
import { HomeComponent } from './home/home.component';

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [ HomeComponent ],
    template: `
    <home></home>
    `,
    styleUrls: ['./app.component.css'],
})
export class AppComponent {
    
}
