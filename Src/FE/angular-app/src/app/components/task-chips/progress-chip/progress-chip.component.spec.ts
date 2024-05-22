import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProgressChipComponent } from './progress-chip.component';

describe('ProgressChipComponent', () => {
  let component: ProgressChipComponent;
  let fixture: ComponentFixture<ProgressChipComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProgressChipComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ProgressChipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
