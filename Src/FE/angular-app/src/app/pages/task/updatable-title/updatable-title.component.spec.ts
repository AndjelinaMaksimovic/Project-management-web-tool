import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdatableTitleComponent } from './updatable-title.component';

describe('UpdatableTitleComponent', () => {
  let component: UpdatableTitleComponent;
  let fixture: ComponentFixture<UpdatableTitleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdatableTitleComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(UpdatableTitleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
