import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GanttDependencyLineComponent } from './gantt-dependency-line.component';

describe('GanttDependencyLineComponent', () => {
  let component: GanttDependencyLineComponent;
  let fixture: ComponentFixture<GanttDependencyLineComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GanttDependencyLineComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(GanttDependencyLineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
