import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GanttSettingsComponent } from './gantt-settings.component';

describe('GanttSettingsComponent', () => {
  let component: GanttSettingsComponent;
  let fixture: ComponentFixture<GanttSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GanttSettingsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(GanttSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
