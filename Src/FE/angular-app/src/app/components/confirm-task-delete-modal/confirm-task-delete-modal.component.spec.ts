import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmTaskDeleteModalComponent } from './confirm-task-delete-modal.component';

describe('ConfirmTaskDeleteModalComponent', () => {
  let component: ConfirmTaskDeleteModalComponent;
  let fixture: ComponentFixture<ConfirmTaskDeleteModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConfirmTaskDeleteModalComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ConfirmTaskDeleteModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
