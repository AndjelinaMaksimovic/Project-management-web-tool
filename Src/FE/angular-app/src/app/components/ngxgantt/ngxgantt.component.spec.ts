import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NgxganttTestComponent } from './ngxgantt.component';

describe('NgxganttTestComponent', () => {
  let component: NgxganttTestComponent;
  let fixture: ComponentFixture<NgxganttTestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NgxganttTestComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(NgxganttTestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
