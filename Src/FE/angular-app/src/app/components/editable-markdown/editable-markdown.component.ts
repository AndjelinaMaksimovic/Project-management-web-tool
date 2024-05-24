import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { MarkdownModule, provideMarkdown } from 'ngx-markdown';
import { CommonModule } from '@angular/common';
import { MarkdownEditorComponent } from '../markdown-editor/markdown-editor.component';

@Component({
  selector: 'app-editable-markdown',
  standalone: true,
  imports: [
    MaterialModule,
    MarkdownModule,
    CommonModule,
    MarkdownEditorComponent,
  ],
  providers: [provideMarkdown()],
  templateUrl: './editable-markdown.component.html',
  styleUrl: './editable-markdown.component.css',
})
export class EditableMarkdownComponent {
  isEditing: boolean = false;
  _content: string | undefined = "";
  @Input() role: any = {}
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
// import { Component, Input } from '@angular/core';
// import { MaterialModule } from '../../material/material.module';
// import { MarkdownModule, provideMarkdown } from 'ngx-markdown';
// import { CommonModule } from '@angular/common';
// import { MarkdownEditorComponent } from '../markdown-editor/markdown-editor.component';

// @Component({
//   selector: 'app-editable-markdown',
//   standalone: true,
//   imports: [
//     MaterialModule,
//     MarkdownModule,
//     CommonModule,
//     MarkdownEditorComponent,
//   ],
//   providers: [provideMarkdown()],
//   templateUrl: './editable-markdown.component.html',
//   styleUrl: './editable-markdown.component.css',
// })
// export class EditableMarkdownComponent {
//   isEditing: boolean = false;
//   @Input() content: string = "";
//   private editContent: string = "";
//   @Input() updateCallback: undefined | ((newContent: string) => unknown);

//   save(){
//     if(this.updateCallback) this.updateCallback(_content);
//     this.isEditing = false;
//   }
// }
