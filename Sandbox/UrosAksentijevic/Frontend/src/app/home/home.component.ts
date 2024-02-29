import { Component, Input, OnInit, } from '@angular/core';
import { ItemComponent } from '../item/item.component';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { environment } from '../../enviroments/enviroment';

import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'home',
    standalone: true,
    imports: [ ItemComponent, FormsModule, HttpClientModule, CommonModule ],
    template: `
        <dialog #myDialog id="modal" class="modal">
            <div class="modal-content">
                <i id="addbtn" (click)="myDialog.close()" class="fa-solid fa-x fa-xl"></i>
                <h2>Izmena:</h2>

                <p>Opis:</p>
                <input [(ngModel)]="updateDesc"/>

                <p>Datum:</p>
                <input [(ngModel)]="updateDate" type="date"/>

                <br><br>

                <button (click)="updateItem();">Izmeni</button>
            </div>
        </dialog>

        <div class="main-container center-container flex-row">
            <h1 style="text-align: center; margin: 20px;">Todo lista</h1>
            <div style="margin: auto; margin-right: 20px;">
                <input [(ngModel)]="desc" style="margin: 10px;" placeholder="Opis zadatka"/>
                <input [(ngModel)]="date" style="margin: 10px;" type="date"/>
                <i id="addbtn" (click)="addItem()" class="fa-solid fa-plus fa-xl" style="margin: 10px;"></i>
            </div>
        </div>

        <div *ngFor="let item of items">
            <item (checkClick)="removeItem(item.id)" (updateClick)="myDialog.show(); editItem(item.id, item.desc, item.date);" (removeClick)="removeItem(item.id)" [desc]="item.desc" [date]="item.date" />
        </div>
    `,
    styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {    
    desc: string = "";
    date: string = "";

    updateId: string = "";
    updateDesc: string = "";
    updateDate: string = "";

    items: any[] = [];

    constructor(private http: HttpClient) { }
    
    addItem() {
        if(this.desc == "") {
            alert("Opis zadatka ne moze biti prazan!");
        }
        else if(this.date == "") {
            alert("Datum mora biti postavljen!");
        }
        else {
            this.http.post<any>(environment.apiUrl + '/Item/AddItem/', { desc: this.desc, date: this.date }).subscribe(() => {
                this.getData();
            });
        }
    }

    removeItem(id : any) {
        this.http.post<any>(environment.apiUrl + '/Item/RemoveItem/' + id, { }).subscribe(() => {
            this.getData();
        });
    }

    editItem(id : any, desc : any, date : any) {
        this.updateId = id;
        this.updateDesc = desc;
        this.updateDate = date;
    }

    updateItem() {
        if(this.updateDesc == "") {
            alert("Opis zadatka ne moze biti prazan!");
        }
        else if(this.updateDate == "") {
            alert("Datum mora biti postavljen!");
        }
        else {
            this.http.put<any>(environment.apiUrl + '/Item/UpdateItem/' + this.updateId, { id: this.updateId, desc: this.updateDesc, date: this.updateDate }).subscribe(() => {
                this.getData();
            });
        }
    }

    ngOnInit() {
        this.getData();
    }

    getData() {
        this.http.get<any>(environment.apiUrl + '/Item/GetItems').subscribe(data => {
            this.items = data;
        });
    }
}
