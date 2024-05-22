import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DependantTasksCardComponent } from './dependant-tasks-card.component';

describe('DependantTasksCardComponent', () => {
  let component: DependantTasksCardComponent;
  let fixture: ComponentFixture<DependantTasksCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DependantTasksCardComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DependantTasksCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
