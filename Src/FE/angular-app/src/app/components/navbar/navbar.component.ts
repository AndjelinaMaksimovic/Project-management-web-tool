import { Component, OnInit, Input } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { NgIf } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon'
import { MatListModule } from '@angular/material/list'
import { animate, state, style, transition, trigger } from '@angular/animations';
import { TopnavComponent } from '../topnav/topnav.component';
import { NgStyle } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ProjectService } from '../../services/project.service';
import { LocalStorageService } from '../../services/localstorage';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [MaterialModule, NgIf, RouterLink, RouterLinkActive, MatSidenavModule, MatIconModule, MatListModule, TopnavComponent, NgStyle ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss',
  animations: [
    trigger('openClose', [
      state('opened', style({
        // width: '100px'
      })),
      state('closed', style({
        width: '60px'
      })),
      transition('opened <=> closed', [animate('0.2s'), ]),
    ])
  ]
})
export class NavbarComponent{
  title?: string
  id?: number
  navMargin = 13;

  constructor(private router: Router, private route: ActivatedRoute, private projectService: ProjectService, private localStorageService: LocalStorageService){}

  ngOnInit(){
    this.route.params.subscribe(async params => {
      this.id = parseInt(params['id']);
      let title = this.projectService.getProjectWithID(this.id)?.title
      if(title)
        this.title = title
      else{
        await this.projectService.fetchProjects({projectId: this.id})
        this.title = this.projectService.getProjectWithID(this.id)?.title
      }
    });

    let expanded = this.localStorageService.getData("navbar_expanded");
    if(expanded && Object.keys(expanded).length === 0 && expanded.constructor === Object) {
      this.localStorageService.saveData("navbar_expanded", true);
      this.isExpanded = true;
    }
    else {
      this.isExpanded = expanded;

      if(this.isExpanded) {
        this.navMargin = 13;
      }
      else {
        this.navMargin = 3.75;
      }
    }
  }
  
  isExpanded = true
  
  toggleNav() {
    this.isExpanded = !this.isExpanded;
    this.localStorageService.saveData("navbar_expanded", this.isExpanded);

    if(this.isExpanded) {
      this.navMargin = 13;
    }
    else {
      this.navMargin = 3.75;
    }
  }
}