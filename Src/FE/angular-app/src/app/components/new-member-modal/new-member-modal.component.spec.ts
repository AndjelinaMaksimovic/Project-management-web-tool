import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewMemberModalComponent } from './new-member-modal.component';

describe('NewMemberModalComponent', () => {
  let component: NewMemberModalComponent;
  let fixture: ComponentFixture<NewMemberModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NewMemberModalComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(NewMemberModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
