import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssigneeChipComponent } from './assignee-chip.component';

describe('AssigneeChipComponent', () => {
  let component: AssigneeChipComponent;
  let fixture: ComponentFixture<AssigneeChipComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AssigneeChipComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AssigneeChipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
