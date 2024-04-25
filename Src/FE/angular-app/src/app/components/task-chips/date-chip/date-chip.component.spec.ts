import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DateChipComponent } from './date-chip.component';

describe('DateChipComponent', () => {
  let component: DateChipComponent;
  let fixture: ComponentFixture<DateChipComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DateChipComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DateChipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
