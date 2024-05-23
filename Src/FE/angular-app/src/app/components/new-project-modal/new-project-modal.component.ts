import { Component } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatListModule } from '@angular/material/list';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { InvitePopupComponent } from '../../components/invite-popup/invite-popup.component';
import { TopnavComponent } from '../../components/topnav/topnav.component';
import {
  MAT_DATE_LOCALE,
  provideNativeDateAdapter,
} from '@angular/material/core';
import { ProjectService } from '../../services/project.service';
import { MatIconModule } from '@angular/material/icon';
import { NgIf } from '@angular/common';
import { MarkdownEditorComponent } from '../markdown-editor/markdown-editor.component';
import moment from 'moment';
import { Router } from '@angular/router';

@Component({
  selector: 'app-new-project-modal',
  standalone: true,
  templateUrl: './new-project-modal.component.html',
  styleUrl: './new-project-modal.component.css',
  imports: [
    MarkdownEditorComponent,
    MaterialModule,
    FormsModule,
    ReactiveFormsModule,
    MatListModule,
    TopnavComponent,
    MatDatepickerModule,
    MatIconModule,
    NgIf,
  ],
})
export class NewProjectModalComponent {
  constructor(
    private dialogue: MatDialog,
    private projectService: ProjectService,
    private dialogRef: MatDialogRef<NewProjectModalComponent>,
    private router: Router,
  ) {}

  projectName = '';
  description = '';
  errorMessage = '';

  currentDate = moment();
  dueDate = new FormControl(this.currentDate);
  startDate = new FormControl(this.currentDate);

  async createNewProject() {
    if (
      !this.projectName ||
      !this.dueDate ||
      !this.description ||
      !this.dueDate.value ||
      !this.startDate.value
    ) {
      this.errorMessage = 'Please provide all required fields';
      return;
    }
    if (
      this.startDate.value.toDate().getTime() >
      this.dueDate.value.toDate().getTime()
    ) {
      this.errorMessage = 'Please enter valid start/due dates';
      return;
    }
    const r = await this.projectService.createNew({
      name: this.projectName,
      description: this.description,
      startDate: this.startDate.value.toDate(),
      dueDate: this.dueDate.value.toDate(),
    });
    this.router.navigateByUrl(`/members/${r.projectId}`);
    return;
  }
}
