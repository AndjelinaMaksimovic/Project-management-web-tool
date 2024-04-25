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
  @Input() value: string | null | undefined = "";
  @Input() label: string = "Content";
  @Output() valueChange = new EventEmitter<string>();
  changeValue(newSelection: string) {
    this.value = newSelection;
    this.valueChange.emit(this.value);
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
     this.changeValue(textarea.value);
  }
}
