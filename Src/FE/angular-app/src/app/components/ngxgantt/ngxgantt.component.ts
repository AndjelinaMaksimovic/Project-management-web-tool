import { DatePipe, NgFor, NgIf } from '@angular/common';
import { AfterViewInit, Component, HostBinding, OnInit, ViewChild, NgModule, Input } from '@angular/core';
import {
    GanttBarClickEvent,
    GanttBaselineItem,
    GanttDragEvent,
    GanttGroup,
    GanttItem,
    GanttItemType,
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
import { CategoryService } from '../../services/category.service';
import { ProjectService } from '../../services/project.service';
import { AvatarService } from '../../services/avatar.service';
import { UserStatsComponent } from '../user-stats/user-stats.component';
import { UserService } from '../../services/user.service';
import { StatusChipComponent } from '../task-chips/status-chip/status-chip.component';
import { PriorityChipComponent } from '../task-chips/priority-chip/priority-chip.component';
import { MatTooltipModule } from '@angular/material/tooltip';

export enum GanttType {
  Projects = 1,
  Tasks = 2
}

export enum ItemType {
  Task = 1,
  Category = 2,
  Milestone = 3,
  Project = 4
}

export class OriginObject {
  task?: any;
  type?: ItemType;
}

@Component({
  selector: 'app-ngxgantt',
  standalone: true,
  imports: [ NgxGanttModule, DatePipe, ThyButtonModule, ThyLayoutModule, ThySwitchModule, FormsModule, NgFor, NgIf, StatusChipComponent, PriorityChipComponent, MatTooltipModule ],
  templateUrl: './ngxgantt.component.html',
  styleUrl: './ngxgantt.component.scss'
})

export class NgxganttComponent {
  GanttType = GanttType;
  ItemType = ItemType;
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

  role: any = {}

  assignedIds: Map<string, []> = new Map<string, []>();

  getAssignedIds(id: string) : [] {
    if(!this.assignedIds.has(id)) return [];
    return this.assignedIds.get(id)!;
  }

  getProfilePicture(id: string) {
    return this.avatarService.getProfileImagePath(id);
  }

  mapTask(task: any): GanttItem {
    // console.log(task);
    // console.log(task.dependentTasks.map((value: { taskId : number, typeOfDependencyId : number }) => {
    //   return { type: value.typeOfDependencyId, link: this.dependencyIdToGanttLink(value.taskId) };
    // }));
    this.assignedIds.set(task.id, task.assignedTo.map((value: any) => value.id));
    return {
      id: task.id,
      title: task.title,
      links: task.dependentTasks.map((value: { taskId : number, typeOfDependencyId : number }) => {
        return { type: this.dependencyIdToGanttLink(value.typeOfDependencyId), link: value.taskId };
      }),
      progress: task.progress / 100.0,
      start: task.startDate,
      end: task.dueDate,
      origin: {
        task: task,
        type: ItemType.Task
      },
      // barStyle: { // MILESTONE STYLE
      //   width: "20px",
      //   height: "20px",
      //   transform: "rotate(45deg)",
      // }
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

  mapProjects(project: any): GanttItem {
    // console.log(project);
    return {
      id: project.id,
      title: project.title,
      progress: project.progress / 100.0,
      start: project.startDate,
      end: project.dueDate,

      expandable: false,
      // draggable: false,
      itemDraggable: false,
      linkable: false,
      origin: {
        type: ItemType.Project
      }
      // color?: string;
      // barStyle?: Partial<CSSStyleDeclaration>;
      // origin?: T;
      // type?: GanttItemType;
    };
  }

  convertTasksToNgx(tasks: any) : GanttItem[] {
    let newTasks = tasks.map((task: any) => {
      return this.mapTask(task);
    });
    return newTasks;
  }

  convertProjectsToNgx(projects: any) : GanttItem[] {
    let newProjects = projects.map((project: any) => {
      return this.mapProjects(project);
    });
    return newProjects;
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
      // {
      //     name: 'week',
      //     value: GanttViewType.week
      // },
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

  items: GanttItem[] = [ ];

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

  @Input() ganttType: GanttType = GanttType.Tasks;

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
    private dialogue: MatDialog,
    private categoryService: CategoryService,
    private projectService: ProjectService,
    private avatarService: AvatarService,
    private userService: UserService
  ) {}

  @Input() projectId: number = -1;

  groups: GanttGroup[] = [];

  async ngOnInit() {
    this.role = await this.userService.currentUserRole();
    // console.log(this.items);

    let ganttView = this.localStorageService.getData("gantt_view");
    if(ganttView && Object.keys(ganttView).length === 0 && ganttView.constructor === Object) {
      this.localStorageService.saveData("gantt_view", this.selectedViewType);
    }
    else {
      this.selectedViewType = this.localStorageService.getData("gantt_view");
      this.selectView(this.selectedViewType);
    }

    if(this.ganttType == GanttType.Tasks) {
      if(this.projectId == -1) {
        await this.taskService.fetchTasksFromLocalStorage(this.projectId, "task_filters");
      }
      // console.log(this.taskService.getTasks());
    }
    else if(this.ganttType == GanttType.Projects) {
      await this.projectService.fetchProjectsLocalStorage("project_filters");
      // console.log(this.projectService.getProjects());
    }
    
    this.updateTasksView();
  }

  async createGroups() {
    let groups: Map<string, GanttItem> = new Map<string, GanttItem>();
    if(this.ganttType == GanttType.Tasks) {
      this.categoryService.setContext({ projectId: this.projectId });
      await this.categoryService.fetchCategories();
      this.categoryService.getCategories().forEach((category) => {
        groups.set(category.id.toString(), {
          id: 'C_'+ category.id.toString(),
          title: category.name,
          itemDraggable: false,
          expanded: true,
          draggable: false,
          linkable: false,
          color: "#bfbfbf",
          children: [],
          start: 0,
          end: 0,
          origin: {
            type: ItemType.Category
          },
          type: GanttItemType.range
        });
      });
    }
    console.log(groups);
    return groups;
  }

  async updateTasksView() {
    if(this.ganttType == GanttType.Tasks) {
      let _groups = await this.createGroups();
      this.taskService.getTasks().forEach(item => {
        let group = _groups.get(item.categoryId.toString());
        group?.children?.push(this.mapTask(item));
      });
      let newItems: Array<GanttItem> = [];
      _groups.forEach(group => {
        if(group.children?.length! > 0) {
          let startDate = group.children!.reduce((minDate, child) => {
              return (new Date(child.start!) < new Date(minDate!)) ? child.start : minDate;
          }, group.children![0].start);
          let endDate = group.children!.reduce((maxDate, child) => {
            return (new Date(child.end!) > new Date(maxDate!)) ? child.end : maxDate;
          }, group.children![0].end);
          group.start = startDate;
          group.end = endDate;
        }
        newItems.push(group);
      });
      this.items = newItems;
      console.log(this.items);
    }
    else if(this.ganttType == GanttType.Projects) {
      this.items = this.convertProjectsToNgx(this.projectService.getProjects());
    }
  }

  ngAfterViewInit() {
      setTimeout(() => this.ganttComponent.scrollToDate(Date.now()), 200);
  }

  barClick(_event: GanttBarClickEvent) {
    const event = _event as GanttBarClickEvent<OriginObject>;

    if(this.ganttType == GanttType.Tasks) {
      if(event.item.origin!.type == ItemType.Task) {
        this.router.navigate(['/project/' + this.projectId + '/task/' + event.item.id]);
        console.log('Event: barClick', `[${event.item.title}]`);
      }
    }
    else if(this.ganttType == GanttType.Projects) {
      this.router.navigate(['/project/' + event.item.id + '/details']);
      console.log('Event: barClick', `[${event.item.title}]`);
    }
  }

  lineClick(event: GanttLineClickEvent) {
    if(this.ganttType == GanttType.Tasks) {
      console.log('Event: lineClick', `Source: [${event.source.title}] Target: [${event.target.title}]`);
      let descriptionMessage = "Are you sure you want to remove the dependency between <b>" + event.source.title + "</b> and <b>" + event.target.title + "</b>?<br>This action cannot be undone and may affect related tasks and workflows.";
      this.dialogue.open(ConfirmationDialogComponent, { data: { title: "Confirm Dependency Removal", description: descriptionMessage, yesFunc: async () => {
        await this.taskService.deleteDependency(parseInt(event.source.id), parseInt(event.target.id)); 
        this.updateTasksView();
      }, noFunc: () => { } } });
    }
  }

  dragMoved(event: GanttDragEvent) {}

  async dragEnded(event: GanttDragEvent) {
    console.log('Event: dragEnded', `[${event.item.title}]`);
    console.log(event.item);

    if(this.ganttType == GanttType.Tasks) {
      console.log((new Date(event.item.start! * 1000 + 6 * 3600 * 1000))); // fix

      await this.taskService.updateTask({
        id: parseInt(event.item.id),
        startDate: (new Date(event.item.start! * 1000 + 6 * 3600 * 1000)), // fix
        dueDate: (new Date(event.item.end! * 1000)),
        forceDateChange: false
      });
    }
    else if(this.ganttType == GanttType.Projects) {
      console.log((new Date(event.item.start! * 1000 + 6 * 3600 * 1000))); // fix

      await this.projectService.updateProject({
        id: parseInt(event.item.id),
        startDate: (new Date(event.item.start! * 1000 + 6 * 3600 * 1000)), // fix
        dueDate: (new Date(event.item.end! * 1000)),
      });
    }
    this.updateTasksView();
  }

  selectedChange(event: GanttSelectedEvent) {
    if(this.ganttType == GanttType.Tasks) {
      event.current && this.ganttComponent.scrollToDate(event.current.start!);

      console.log(
          'Event: selectedChange',
          `Task ids: ${(event.selectedValue as GanttItem[]).map((item) => item.id).join('、')}`
      );
    }
  }

  async linkDragEnded(event: GanttLinkDragEvent) {
    if(this.ganttType == GanttType.Tasks) {
      console.log('Event: linkDragEnded', `Source: [${event.source.title}] Target: [${event.target!.title}]`);
      let type = this.GanttLinkToDependencyId(event.type!);
      console.log(type);
      await this.taskService.createTaskDependency({ taskId: parseInt(event.source!.id), dependentTaskId: parseInt(event.target!.id), typeOfDependencyId: type });
      this.updateTasksView();
    }
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
      this.localStorageService.saveData("gantt_view", event.viewType);
  }

  async onDragDropped(event: GanttTableDragDroppedEvent) {
    if(this.ganttType == GanttType.Tasks) {
      console.log(event);
      let categoryId = -1;
      if(event.targetParent == undefined) {
        categoryId = parseInt(event.target.id.replace("C_", ""));
      }
      else {
        categoryId = parseInt(event.targetParent.id.replace("C_", ""));
      }
      await this.taskService.updateTask({
        id: parseInt(event.source.id),
        categoryId: categoryId
      });
      this.updateTasksView();
    }
  }

  onDragStarted(event: GanttTableDragStartedEvent) {
    if(this.ganttType == GanttType.Tasks) {
      console.log('Index drag started', event);
    }
  }

  onDragEnded(event: GanttTableDragEndedEvent) {
    if(this.ganttType == GanttType.Tasks) {
      console.log('Index drag ended', event);
    }
  }
}
