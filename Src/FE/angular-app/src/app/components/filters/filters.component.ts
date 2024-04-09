import { Component, Input } from '@angular/core';
import { MatSelectModule } from '@angular/material/select';
import { NgFor } from '@angular/common';
import { MatMenuModule } from '@angular/material/menu';
import { CommonModule } from '@angular/common';
import { NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';

class Item {
  value: string;
  name: string;

  constructor(item?: any) {
    this.value = (item && item.value) || null;
    this.name = (item && item.name) || null;
  }
}

class Filter {
  value: string;
  icon: string;
  name: string;
  filterName: string;
  type: string;
  items: Array<any>;
  enabled: boolean;

  constructor(filter?: any) {
    this.value = (filter && filter.value) || null;
    this.icon = (filter && filter.icon) || null;
    this.name = (filter && filter.name) || null;
    this.filterName = (filter && filter.filterName) || null;
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
  @Input() title: string = "";
  currentFilters: Map<number, boolean> = new Map();

  @Input() allFilters: Filter[] = [
    new Filter({ filterName: "DueDateAfter", name: 'Start date', icon: 'fa-regular fa-calendar', type: 'date' }),
    new Filter({ filterName: "DueDateBefore", name: 'Due date', icon: 'fa-solid fa-flag-checkered', type: 'date' }),
    new Filter({ filterName: "AssignedTo", name: 'Assigned to', icon: 'fa-solid fa-user', type: 'select', items: [ new Item({ value: "1", name: "Test" }) ] }),
  ];
  
  constructor() {
    this.allFilters.map((item, index) => ({ ...item, id: index + 1 }));
  }

  addFilter(filter : Filter, i : number) {
    this.currentFilters.set(i, true);
    this.allFilters[i].enabled = false;
  }

  removeFilter(i : number) {
    this.currentFilters.delete(i);
    this.allFilters[i].enabled = true;
  }

  searchUser(term: string, item: any) {
    item.name = item.name.replace(',','');
    term = term.toLocaleLowerCase();
    return item.name.toLocaleLowerCase().indexOf(term) > -1;
  }

  onChange(filterKey : number, event : any) {
    let newValue = event.target.value;
    this.allFilters[filterKey].value = newValue;
  }

  onChangeSelection(filterKey : number, event : any) {
    let newValue = event.value;
    this.allFilters[filterKey].value = newValue;
  }

  save() {
    let newFilters: Map<string, string> = new Map<string, string>();
    for(let [key, value] of this.currentFilters) {
      let filterKey = this.allFilters[key].filterName;
      newFilters.set(filterKey, this.allFilters[key].value);
    }
    const obj = Object.fromEntries(newFilters);
    return obj;
  }
}
