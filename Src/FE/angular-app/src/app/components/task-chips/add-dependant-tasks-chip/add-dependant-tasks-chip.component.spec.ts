import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddDependantTasksChipComponent } from './add-dependant-tasks-chip.component';

describe('AddDependantTasksChipComponent', () => {
  let component: AddDependantTasksChipComponent;
  let fixture: ComponentFixture<AddDependantTasksChipComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddDependantTasksChipComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AddDependantTasksChipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
