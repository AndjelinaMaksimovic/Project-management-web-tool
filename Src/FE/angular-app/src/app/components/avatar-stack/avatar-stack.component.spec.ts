import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AvatarStackComponent } from './avatar-stack.component';

describe('AvatarStackComponent', () => {
  let component: AvatarStackComponent;
  let fixture: ComponentFixture<AvatarStackComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AvatarStackComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AvatarStackComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
