import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MarkdownModule, provideMarkdown } from 'ngx-markdown';
import { MaterialModule } from '../../material/material.module';
import { MarkdownEditorComponent } from '../markdown-editor/markdown-editor.component';

@Component({
  selector: 'app-markdown-edit-chip',
  standalone: true,
  imports: [
    MaterialModule,
    MarkdownModule,
    CommonModule,
    MarkdownEditorComponent,
  ],
  providers: [provideMarkdown()],
  templateUrl: './markdown-edit-chip.component.html',
  styleUrl: './markdown-edit-chip.component.css'
})
export class MarkdownEditChipComponent {
  isEditing: boolean = false;
  _content: string | undefined = "";
  @Input({required: true}) role: any = {}
  get content(){
    return this._content;
  }
  @Input() set content(newContent: string | undefined){
    if(!newContent) return;
    this._content = newContent;
    this.editContent = newContent;
  }
  public editContent: string = "";

  @Output() onSave: EventEmitter<string> = new EventEmitter();

  save(){
    this.onSave.emit(this.editContent);
    this.isEditing = false;
  }
  cancel(){
    this.isEditing = false;
    this.editContent = this.content || "";
  }
}
