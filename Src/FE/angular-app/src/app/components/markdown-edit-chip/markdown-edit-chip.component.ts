import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { MarkdownModule, provideMarkdown } from 'ngx-markdown';
import { MaterialModule } from '../../material/material.module';
import { MarkdownEditorComponent } from '../markdown-editor/markdown-editor.component';
import { MatMenuTrigger } from '@angular/material/menu';
import { MatDialog } from '@angular/material/dialog';
import { EditableMarkdownComponent } from '../editable-markdown/editable-markdown.component';
import { EditableMarkdownModalComponent } from './editable-markdown-modal/editable-markdown-modal.component';

@Component({
  selector: 'app-markdown-edit-chip',
  standalone: true,
  imports: [
    MaterialModule,
    MarkdownModule,
    CommonModule,
    MarkdownEditorComponent,
    MatMenuTrigger,
  ],
  providers: [provideMarkdown()],
  templateUrl: './markdown-edit-chip.component.html',
  styleUrl: './markdown-edit-chip.component.css'
})
export class MarkdownEditChipComponent {
  @Input() content: string = ''
  @Input({required: true}) role: any = {}
  @Output() onSave: EventEmitter<string> = new EventEmitter();

  constructor(private dialog: MatDialog){}

  openModal(){
    const ref = this.dialog.open(EditableMarkdownModalComponent, {
      autoFocus: false,
      data: {
        content: this.content,
        role: this.role
      }
    })
    ref.afterClosed().subscribe(data => {
      if(data){
        this.onSave.emit(data)
      }
    })
  }
}
