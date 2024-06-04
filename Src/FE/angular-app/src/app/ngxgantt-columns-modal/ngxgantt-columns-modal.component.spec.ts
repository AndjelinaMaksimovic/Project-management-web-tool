import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NgxganttColumnsModalComponent } from './ngxgantt-columns-modal.component';

describe('NgxganttColumnsModalComponent', () => {
  let component: NgxganttColumnsModalComponent;
  let fixture: ComponentFixture<NgxganttColumnsModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NgxganttColumnsModalComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(NgxganttColumnsModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
