import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditableMarkdownModalComponent } from './editable-markdown-modal.component';

describe('EditableMarkdownModalComponent', () => {
  let component: EditableMarkdownModalComponent;
  let fixture: ComponentFixture<EditableMarkdownModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditableMarkdownModalComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(EditableMarkdownModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
