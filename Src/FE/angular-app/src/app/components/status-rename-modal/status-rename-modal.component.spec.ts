import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StatusRenameModalComponent } from './status-rename-modal.component';

describe('StatusRenameModalComponent', () => {
  let component: StatusRenameModalComponent;
  let fixture: ComponentFixture<StatusRenameModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StatusRenameModalComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(StatusRenameModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
