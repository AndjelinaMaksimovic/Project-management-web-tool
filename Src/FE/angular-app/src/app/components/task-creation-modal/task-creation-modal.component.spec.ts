import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TaskCreationModalComponent } from './task-creation-modal.component';

describe('TaskCreationModalComponent', () => {
  let component: TaskCreationModalComponent;
  let fixture: ComponentFixture<TaskCreationModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaskCreationModalComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(TaskCreationModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
