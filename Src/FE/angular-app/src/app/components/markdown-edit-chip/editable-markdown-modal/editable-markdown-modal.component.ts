import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Inject, Input, Output, ViewChild } from '@angular/core';
import { MatMenuTrigger } from '@angular/material/menu';
import { MarkdownModule } from 'ngx-markdown';
import { MaterialModule } from '../../../material/material.module';
import { MarkdownEditorComponent } from '../../markdown-editor/markdown-editor.component';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

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

  constructor(@Inject(MAT_DIALOG_DATA) public data: any, private dialogRef: MatDialogRef<EditableMarkdownModalComponent>){
    this.content = data.content
  }

  save(){
    // this.onSave.emit(this.editContent);
    this.isEditing = false;
    this.dialogRef.close(this.editContent)
  }
  cancel(){
    this.isEditing = false;
    this.editContent = this.content || "";
    this.dialogRef.close()
  }
}
