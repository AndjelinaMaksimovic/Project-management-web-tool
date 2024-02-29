import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
    selector: 'item',
    standalone: true,
    imports: [],
    template: `
        <div class="main-container center-container flex-row">
            <div>
                <p style="text-align: left; margin: 20px;">{{desc}}</p>
                <p style="text-align: left; margin: 20px;"><b>{{date}}</b></p>
            </div>            
            <div style="min-width: 200px; margin-top: auto; margin-bottom: auto;">
                <i (click)="this.checkClick.emit();" class="fa-solid fa-check" style="margin: 20px; color: green;"></i>
                <i (click)="this.updateClick.emit();" class="fa-regular fa-pen-to-square" style="margin: 20px;"></i>
                <i (click)="this.removeClick.emit();" class="fa-regular fa-trash-can" style="margin: 20px; color: red;"></i>
            </div>
        </div>
    `,
    styleUrl: './item.component.css'
})
export class ItemComponent {
    @Input() desc: string = "default";
    @Input() date: string = "default";

    @Output() removeClick = new EventEmitter<void>();
    @Output() updateClick = new EventEmitter<void>();
    @Output() checkClick = new EventEmitter<void>();
}
