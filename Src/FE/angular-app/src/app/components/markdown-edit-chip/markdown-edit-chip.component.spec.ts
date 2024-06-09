import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MarkdownEditChipComponent } from './markdown-edit-chip.component';

describe('MarkdownEditChipComponent', () => {
  let component: MarkdownEditChipComponent;
  let fixture: ComponentFixture<MarkdownEditChipComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MarkdownEditChipComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(MarkdownEditChipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
