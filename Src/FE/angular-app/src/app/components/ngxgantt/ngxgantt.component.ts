import { DatePipe, NgFor } from '@angular/common';
import { AfterViewInit, Component, HostBinding, OnInit, ViewChild, NgModule, Input } from '@angular/core';
import {
    GanttBarClickEvent,
    GanttBaselineItem,
    GanttDragEvent,
    GanttItem,
    GanttLineClickEvent,
    GanttLinkDragEvent,
    GanttLinkLineType,
    GanttLinkType,
    GanttSelectedEvent,
    GanttTableDragDroppedEvent,
    GanttTableDragEndedEvent,
    GanttTableDragEnterPredicateContext,
    GanttTableDragStartedEvent,
    GanttToolbarOptions,
    GanttView,
    GanttViewType,
    NgxGanttComponent,
    NgxGanttModule
} from '@worktile/gantt';
import { TaskService } from '../../services/task.service';
import { StatusService } from '../../services/status.service';
import { PriorityService } from '../../services/priority.service';
import { ActivatedRoute, Router } from '@angular/router';
import { LocalStorageService } from '../../services/localstorage';
import { ThyButtonModule } from 'ngx-tethys/button';
import { ThyLayoutModule } from 'ngx-tethys/layout';
import { ThySwitchModule } from 'ngx-tethys/switch';
import { FormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from '../confirmation-dialog/confirmation-dialog.component';

@Component({
  selector: 'app-ngxgantt',
  standalone: true,
  imports: [ NgxGanttModule, DatePipe, ThyButtonModule, ThyLayoutModule, ThySwitchModule, FormsModule, NgFor ],
  templateUrl: './ngxgantt.component.html',
  styleUrl: './ngxgantt.component.scss'
})

export class NgxganttComponent {
  GanttLinkToDependencyId(type: GanttLinkType) {
    let newType = -1;
      switch(type) {
        case GanttLinkType.ss:
          newType = 1
          break;
        case GanttLinkType.sf:
          newType = 2
          break;
        case GanttLinkType.fs:
          newType = 3
          break;
        case GanttLinkType.ff:
          newType = 4
          break;
      }
    return newType;
  }

  dependencyIdToGanttLink(type: number) {
    switch(type) {
      case 1:
        return GanttLinkType.ss
      case 2:
        return GanttLinkType.sf
      case 3:
        return GanttLinkType.fs
      case 4:
        return GanttLinkType.ff
    }
    return -1;
  }

  mapTask(task: any): GanttItem {
    // console.log(task.dependentTasks.map((value: { taskId : number, typeOfDependencyId : number }) => {
    //   return { type: value.typeOfDependencyId, link: this.dependencyIdToGanttLink(value.taskId) };
    // }));
    return {
      id: task.id,
      title: task.title,
      links: task.dependentTasks.map((value: { taskId : number, typeOfDependencyId : number }) => {
        return { type: this.dependencyIdToGanttLink(value.typeOfDependencyId), link: value.taskId };
      }),
      start: task.startDate,
      end: task.dueDate,
      // links: task.dependentTasks.foreach()
    //   itemDraggable: false

    //   expandable: false,
      // group_id: string;
      // links?: (GanttLink | string)[];
      // draggable?: boolean;
      // itemDraggable?: boolean;
      // linkable?: boolean;
      // expanded?: boolean;
      // children?: GanttItem[];
      // color?: string;
      // barStyle?: Partial<CSSStyleDeclaration>;
      // origin?: T;
      // type?: GanttItemType;
      // progress?: number;
    };
  }

  convertTasksToNgx(tasks: any) : GanttItem[] {
    let newTasks = tasks.map((task: any) => {
      return this.mapTask(task);
    });
    return newTasks;
  }

  views = [
      // {
      //     name: 'h',
      //     value: GanttViewType.hour
      // },
      {
          name: 'day',
          value: GanttViewType.day
      },
      {
          name: 'week',
          value: GanttViewType.week
      },
      {
          name: 'month',
          value: GanttViewType.month
      },
      {
          name: 'Q',
          value: GanttViewType.quarter
      },
      {
          name: 'year',
          value: GanttViewType.year
      }
  ];

  viewType: GanttViewType = GanttViewType.day;

  selectedViewType: GanttViewType = GanttViewType.day;

  isBaselineChecked = false;

  isShowToolbarChecked = true;

  loading = false;

  items: GanttItem[] = [
      // { id: '000001', title: 'Task 1', start: Date.now(), end: Date.now() + 4 * 24 * 60 * 60 * 1000, links: ['000002'], progress: 1, itemDraggable: false, color: '#000' },
      // { id: '000002', title: 'Task 2', start: Date.now(), end: Date.now() + 5 * 24 * 60 * 60 * 1000, links: [], progress: 1, itemDraggable: false, color: '#709dc1' },
  ];

  toolbarOptions: GanttToolbarOptions = {
      viewTypes: [GanttViewType.day, GanttViewType.month, GanttViewType.year]
  };

  baselineItems: GanttBaselineItem[] = [];

  options = {
      viewType: GanttViewType.day
  };

  viewOptions = {
    dateFormat: {
      hour: 'HH:mm',
      day: 'M/d',
      week: 'w',
      month: 'MMMM',
      quarter: 'QQQ',
      year: 'yyyy',
      yearMonth: 'yyyy MMMM',
      yearQuarter: 'yyyy QQQ'
    },
  };

  linkOptions = {
    dependencyTypes: [GanttLinkType.ff, GanttLinkType.fs, GanttLinkType.sf, GanttLinkType.ss],
    showArrow: true,
    lineType: GanttLinkLineType.curve
  };
  
  styles = {
    lineHeight: 44,
    barHeight: 22
  };

  @HostBinding('class.gantt-example-component') class = true;

  @ViewChild('gantt') ganttComponent!: NgxGanttComponent;

  dropEnterPredicate = (event: GanttTableDragEnterPredicateContext) => {
      return true;
  };

  constructor(
    private taskService: TaskService,
    private statusService: StatusService,
    private priorityService: PriorityService,
    private route: ActivatedRoute,
    private router: Router,
    private localStorageService: LocalStorageService,
    private dialogue: MatDialog
  ) {}

  @Input() projectId: number = -1;

  async ngOnInit() {
    console.log(this.items);

    if(this.projectId == -1) {
      await this.taskService.fetchTasksFromLocalStorage(this.projectId, "task_filters");
    }
    this.updateTasksView();
    console.log(this.taskService.getTasks());
  }

  updateTasksView() {
    this.items = this.convertTasksToNgx(this.taskService.getTasks());
  }

  ngAfterViewInit() {
      setTimeout(() => this.ganttComponent.scrollToDate(Date.now()), 200);
  }

  barClick(event: GanttBarClickEvent) {
      console.log('Event: barClick', `[${event.item.title}]`);
  }

  lineClick(event: GanttLineClickEvent) {
    console.log('Event: lineClick', `Source: [${event.source.title}] Target: [${event.target.title}]`);
    let descriptionMessage = "Are you sure you want to remove the dependency between <b>" + event.source.title + "</b> and <b>" + event.target.title + "</b>?<br>This action cannot be undone and may affect related tasks and workflows.";
    this.dialogue.open(ConfirmationDialogComponent, { data: { title: "Confirm Dependency Removal", description: descriptionMessage, yesFunc: async () => {
      await this.taskService.deleteDependency(parseInt(event.source.id), parseInt(event.target.id)); 
      this.updateTasksView();
    }, noFunc: () => { } } });
  }

  dragMoved(event: GanttDragEvent) {}

  async dragEnded(event: GanttDragEvent) {
      console.log('Event: dragEnded', `[${event.item.title}]`);

      console.log(event.item);

      console.log((new Date(event.item.start! * 1000 + 6 * 3600 * 1000))); // fix

      await this.taskService.updateTask({
        id: parseInt(event.item.id),
        startDate: (new Date(event.item.start! * 1000 + 6 * 3600 * 1000)), // fix
        dueDate: (new Date(event.item.end! * 1000)),
        forceDateChange: false
      });
      this.updateTasksView();
  }

  selectedChange(event: GanttSelectedEvent) {
      event.current && this.ganttComponent.scrollToDate(event.current.start!);

      console.log(
          'Event: selectedChange',
          `Task ids: ${(event.selectedValue as GanttItem[]).map((item) => item.id).join('、')}`
      );
  }

  async linkDragEnded(event: GanttLinkDragEvent) {
      console.log('Event: linkDragEnded', `Source: [${event.source.title}] Target: [${event.target!.title}]`);
      let type = this.GanttLinkToDependencyId(event.type!);
      console.log(type);
      await this.taskService.createTaskDependency({ taskId: parseInt(event.source!.id), dependentTaskId: parseInt(event.target!.id), typeOfDependencyId: type });
      this.updateTasksView();
  }

  scrollToToday() {
      this.ganttComponent.scrollToToday();
  }

  selectView(type: GanttViewType) {
      this.viewType = type;
      this.selectedViewType = type;
  }

  viewChange(event: GanttView) {
      console.log(event.viewType);
      this.selectedViewType = event.viewType;
  }

  onDragDropped(event: GanttTableDragDroppedEvent) {
    //   const sourceItems = event.sourceParent?.children || this.items;
    //   sourceItems.splice(sourceItems.indexOf(event.source), 1);
    //   if (event.dropPosition === 'inside') {
    //       event.target.children = [...(event.target.children || []), event.source];
    //   } else {
    //       const targetItems = event.targetParent?.children || this.items;
    //       if (event.dropPosition === 'before') {
    //           targetItems.splice(targetItems.indexOf(event.target), 0, event.source);
    //       } else {
    //           targetItems.splice(targetItems.indexOf(event.target) + 1, 0, event.source);
    //       }
    //   }
      this.items = [...this.items];
  }

  onDragStarted(event: GanttTableDragStartedEvent) {
      console.log('Index drag started', event);
  }

  onDragEnded(event: GanttTableDragEndedEvent) {
      console.log('Index drag ended', event);
  }
}
