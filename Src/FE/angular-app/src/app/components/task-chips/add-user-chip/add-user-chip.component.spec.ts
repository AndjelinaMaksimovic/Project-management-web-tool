import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddUserChipComponent } from './add-user-chip.component';

describe('AddUserChipComponent', () => {
  let component: AddUserChipComponent;
  let fixture: ComponentFixture<AddUserChipComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddUserChipComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AddUserChipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
