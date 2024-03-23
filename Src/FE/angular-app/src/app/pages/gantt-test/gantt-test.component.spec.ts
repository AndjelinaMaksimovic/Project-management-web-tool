import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GanttTestComponent } from './gantt-test.component';

describe('GanttTestComponent', () => {
  let component: GanttTestComponent;
  let fixture: ComponentFixture<GanttTestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GanttTestComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(GanttTestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
