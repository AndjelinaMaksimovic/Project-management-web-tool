import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InviteToProjectModalComponent } from './invite-to-project-modal.component';

describe('InviteToProjectModalComponent', () => {
  let component: InviteToProjectModalComponent;
  let fixture: ComponentFixture<InviteToProjectModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InviteToProjectModalComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(InviteToProjectModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
