import { Component, ViewChild, ElementRef, Input, Output, EventEmitter } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MarkdownModule, provideMarkdown } from 'ngx-markdown';
@Component({
  selector: 'app-markdown-editor',
  standalone: true,
  imports: [MaterialModule, FormsModule, ReactiveFormsModule, CommonModule, MarkdownModule],
  providers: [provideMarkdown()],
  templateUrl: './markdown-editor.component.html',
  styleUrl: './markdown-editor.component.css',
})
export class MarkdownEditorComponent {
  @Input() description: string | null | undefined = "";
  @Output() descriptionChange = new EventEmitter<string>();
  changeSelection(newSelection: string) {
    this.description = newSelection;
    this.descriptionChange.emit(this.description);
  }

  @ViewChild('editor') editor!: ElementRef;

  insertText(before: string, after: string) {
     const textarea = this.editor.nativeElement;
     const start = textarea.selectionStart;
     const end = textarea.selectionEnd;
     const textBefore = textarea.value.substring(0, start);
     const textAfter = textarea.value.substring(end);
     const selectedText = textarea.value.substring(start, end);
     const newText = `${textBefore}${before}${selectedText}${after}${textAfter}`;
     textarea.value = newText;
     textarea.selectionStart = start + before.length;
     textarea.selectionEnd = end + before.length;
  }
}
