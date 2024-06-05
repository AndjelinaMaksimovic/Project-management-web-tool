import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { MatMenuTrigger } from '@angular/material/menu';
import { MarkdownModule } from 'ngx-markdown';
import { MaterialModule } from '../../../material/material.module';
import { MarkdownEditorComponent } from '../../markdown-editor/markdown-editor.component';

@Component({
  selector: 'app-editable-markdown-modal',
  standalone: true,
  imports: [
    MaterialModule,
    MarkdownModule,
    CommonModule,
    MarkdownEditorComponent,
  ],
  templateUrl: './editable-markdown-modal.component.html',
  styleUrl: './editable-markdown-modal.component.css'
})
export class EditableMarkdownModalComponent {
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
  @ViewChild('menuTrigger') trigger!: MatMenuTrigger;

  save(){
    this.onSave.emit(this.editContent);
    this.isEditing = false;
    this.trigger.closeMenu();
  }
  cancel(){
    this.isEditing = false;
    this.editContent = this.content || "";
    this.trigger.closeMenu();
  }
}
