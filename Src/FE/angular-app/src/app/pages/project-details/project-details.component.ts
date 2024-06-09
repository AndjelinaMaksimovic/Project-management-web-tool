import { Component, Input } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { ProjectItemComponent } from '../../components/project-item/project-item.component';
import { NgFor, NgIf } from '@angular/common';
import { StatusItemComponent } from '../../components/status-item/status-item.component';
import { ProgressbarComponent } from '../../components/progressbar/progressbar.component';
import { ActivityItemComponent } from '../../components/activity-item/activity-item.component';
import { Project, ProjectService } from '../../services/project.service';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';
import { TaskService } from '../../services/task.service';
import { MatDialog } from '@angular/material/dialog';
import { CreateStatusModalComponent } from '../../components/create-status-modal/create-status-modal.component';
import { CreateCategoryModalComponent } from '../../components/create-category-modal/create-category-modal.component';
import { CategoryService } from '../../services/category.service';
import { StatusService } from '../../services/status.service';
import { MatTooltipModule } from '@angular/material/tooltip';
import { UserService } from '../../services/user.service';
import { AvatarService } from '../../services/avatar.service';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { EditableMarkdownComponent } from '../../components/editable-markdown/editable-markdown.component';
import { provideMarkdown } from 'ngx-markdown';
import { UpdatableTitleComponent } from '../task/updatable-title/updatable-title.component';
import { MatMenuModule } from '@angular/material/menu';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../../material/material.module';
import moment from 'moment';
import { DateAdapter, MAT_DATE_LOCALE } from '@angular/material/core';
import { MAT_MOMENT_DATE_ADAPTER_OPTIONS, MomentDateAdapter } from '@angular/material-moment-adapter';
import { MarkdownEditChipComponent } from '../../components/markdown-edit-chip/markdown-edit-chip.component';

@Component({
  selector: 'app-project-details',
  standalone: true,
  imports: [ NavbarComponent, ProjectItemComponent, NgIf, StatusItemComponent, ProgressbarComponent, ActivityItemComponent, DatePipe, NgFor, MatTooltipModule, MatPaginatorModule, EditableMarkdownComponent, UpdatableTitleComponent, MaterialModule, MatMenuModule, FormsModule, ReactiveFormsModule, MarkdownEditChipComponent ],
  providers: [
    provideMarkdown(),
    // {provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS]},
    // {provide: MAT_MOMENT_DATE_ADAPTER_OPTIONS, useValue: {useUtc: true}}
  ],
  templateUrl: './project-details.component.html',
  styleUrl: './project-details.component.scss'
})
export class ProjectDetailsComponent {
  project?: Project;

  activities: any[] = []
  viewActivities: any[] = []
  paginatorLen = 0
  paginatorPageSize = 5

  projectId: number = 0;
  title?: string = "";
  description: string = "";
  dueDate?: Date = new Date();
  daysLeft : number = 0;
  progress : number = 0;

  allTasks : number = 0;
  completedTasks : number = 0;
  overdueTasks : number = 0;

  role: any = {}
  tempDueDate = new FormControl(moment());

  constructor(private projectService: ProjectService, private route: ActivatedRoute, private taskService: TaskService, public dialog: MatDialog, private categoryService : CategoryService, private statusService : StatusService, private userService: UserService, private avatarService: AvatarService) {
    this.dialog.closeAll();
  }

  get statuses() {
    return this.statusService.getStatuses();
  }

  get categories() {
    return this.categoryService.getCategories();
  }

  async ngOnInit() {
    this.route.params.subscribe((params) => {
      this.projectId = parseInt(params['id']);
    });
    await this.projectService.fetchProjects();
    this.project = this.projectService.getProjectWithID(this.projectId);

    await this.taskService.fetchTasks({ projectId: this.projectId });
    await this.statusService.fetchStatuses();

    this.title = this.project?.title;
    this.description = this.project?.description ?? '';
    this.dueDate = this.project?.dueDate;

    let difference = this.dueDate!.getTime() - new Date().getTime();
    this.daysLeft = Math.floor(difference / (1000 * 60 * 60 * 24));

    const _progress = await this.projectService.getProjectProgress(this.projectId);
    if(_progress != null && _progress != undefined) {
      this.progress = _progress;
    }
    this.allTasks = this.taskService.getTasks().length;
    this.completedTasks = this.taskService.getTasks().filter((task) => task.status == "Done").length;
    this.overdueTasks = this.taskService.getTasks().filter((task) => new Date(task.dueDate) < new Date()).length;


    this.activities = await this.projectService.allProjectActivities(this.projectId)
    this.activities = this.activities.sort((a: any, b: any) => a.time > b.time ? -1 : 1)
    this.paginatorLen = this.activities.length
    this.viewActivities = this.activities.slice(0, this.paginatorPageSize)

    this.role = await this.userService.currentUserRole(this.projectId)
    
    console.log(this.taskService.getTasks());
  }

  createStatus() {
    this.dialog.open(CreateStatusModalComponent);
  }

  createCategory() {
    this.dialog.open(CreateCategoryModalComponent);
  }

  deleteStatus(status: string) {
    this.statusService.deleteStatus(status);
  }

  deleteCategory(category: number) {
    this.categoryService.deleteCategory(category);
  }

  pageChange(e: PageEvent){
    const offset = e.pageIndex * e.pageSize
    this.viewActivities = this.activities.slice(offset, offset + e.pageSize)
  }

  updateDescription(newDescription: string) {
    this.description = newDescription
    this.projectService.updateProject({
      id: this.projectId,
      description: newDescription,
    });
  }

  setTmpDueDate(){
    this.tempDueDate.setValue(moment(this.dueDate))
    const offset = this.tempDueDate.value!.utcOffset()
    this.tempDueDate.value?.add(moment.duration(offset, 'minutes'))
  }

  updateDate() {
    // if(!this.tempDueDate)
    //   return

    // datepicker offsets chosen day by timezone, so add offset to make it UTC
    const offset = this.tempDueDate.value!.utcOffset()
    this.tempDueDate.value?.add(moment.duration(offset, 'minutes'))
    this.dueDate = moment(this.tempDueDate.value).toDate()

    let difference = this.dueDate!.getTime() - new Date().getTime();
    this.daysLeft = Math.floor(difference / (1000 * 60 * 60 * 24));

    this.projectService.updateProject({
      id: this.projectId,
      dueDate: this.dueDate,
    });
  }
}
