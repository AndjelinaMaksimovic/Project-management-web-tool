import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateStatusModalComponent } from './create-status-modal.component';

describe('CreateStatusModalComponent', () => {
  let component: CreateStatusModalComponent;
  let fixture: ComponentFixture<CreateStatusModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateStatusModalComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CreateStatusModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
