import { Component, Input, Output } from '@angular/core';
import { MatSelectModule } from '@angular/material/select';
import { NgFor } from '@angular/common';
import { MatMenuModule } from '@angular/material/menu';
import { CommonModule } from '@angular/common';
import { NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LocalStorageService } from '../../services/localstorage';
import { EventEmitter } from '@angular/core';

export class Filter {
  value: any;
  icon: string;
  name: string;
  type: string;
  items: Array<any>;
  enabled: boolean;

  constructor(filter?: any) {
    this.value = (filter && filter.value) || null;
    this.icon = (filter && filter.icon) || null;
    this.name = (filter && filter.name) || null;
    this.type = (filter && filter.type) || "";
    this.items = (filter && filter.items) || [];
    this.enabled = (filter && filter.enabled) || true;
  }
}

@Component({
  selector: 'app-filters',
  standalone: true,
  imports: [ MatSelectModule, NgFor, NgIf, MatMenuModule, CommonModule, FormsModule ],
  templateUrl: './filters.component.html',
  styleUrl: './filters.component.css'
})
export class FiltersComponent {
  @Input() filtersName: string = "";
  @Input() title: string = "";
  @Output() fetchProjects: EventEmitter<any> = new EventEmitter();
  @Output() filterOpenEvent = new EventEmitter<boolean>();

  currentFilters: Map<string, boolean> = new Map();

  @Input() allFilters: Map<string, Filter> = new Map<string, Filter>();
  
  constructor(private localStorageService: LocalStorageService) {
    
  }

  ngOnInit() {
    let storageFilters = new Map(Object.entries(this.localStorageService.getData(this.filtersName)));

    if(storageFilters != null) {
      for(let [key, value] of storageFilters as Map<string, string>) {
        if(this.allFilters.has(key)) {
          this.allFilters.get(key)!.value = value;
          this.addFilter(key);
        }
      }
    }
  }

  addFilter(key : string) {
    let filter = this.allFilters.get(key);
    if(filter) {
      filter.enabled = false;
      this.currentFilters.set(key, true);
    }
  }

  removeFilter(key : string) {
    let filter = this.allFilters.get(key);
    if(filter) {
      filter.enabled = true;
      if(filter.type != "bool") filter.value = "";
      this.currentFilters.delete(key);
    }
  }

  clearAllFilters() {
    this.currentFilters.clear();
    this.allFilters.forEach((filter, key) => {
      filter.enabled = true;
      if(filter.type != "bool") filter.value = "";
    });
  }

  save() {
    let newFilters: Map<string, string> = new Map<string, string>();
    for(let [key, value] of this.currentFilters) {
      console.log(key + " " + this.allFilters.get(key)!.value);
      if(this.allFilters.get(key)!.value != null && this.allFilters.get(key)!.value != undefined && (key == 'AssignedTo' || this.allFilters.get(key)!.value != '')) {
        newFilters.set(key, this.allFilters.get(key)!.value);
      }
      else {
        this.removeFilter(key);
      }
    }
    const obj = Object.fromEntries(newFilters);
    this.localStorageService.saveData(this.filtersName, obj);

    this.fetchProjects.emit();

    this.closeFilter();
  }

  closeFilter() {
    this.filterOpenEvent.emit(false);
  }
}
