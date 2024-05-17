import { DatePipe } from '@angular/common';
import { AfterViewInit, Component, HostBinding, OnInit, ViewChild } from '@angular/core';
import {
    GanttBarClickEvent,
    GanttBaselineItem,
    GanttDragEvent,
    GanttItem,
    GanttLineClickEvent,
    GanttLinkDragEvent,
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

@Component({
  selector: 'app-ngxgantt-test',
  standalone: true,
  imports: [ NgxGanttModule, DatePipe ],
  templateUrl: './ngxgantt-test.component.html',
  styleUrl: './ngxgantt-test.component.scss'
})

export class NgxganttTestComponent {
  views = [
      {
          name: 'h',
          value: GanttViewType.hour
      },
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
  
  styles = {
    lineHeight: 33,
    barHeight: 22
  };

  @HostBinding('class.gantt-example-component') class = true;

  @ViewChild('gantt') ganttComponent!: NgxGanttComponent;

  dropEnterPredicate = (event: GanttTableDragEnterPredicateContext) => {
      return true;
  };

  ngOnInit(): void {
      console.log(this.items);
  }

  ngAfterViewInit() {
      setTimeout(() => this.ganttComponent.scrollToDate(Date.now()), 200);
  }

  barClick(event: GanttBarClickEvent) {
      console.log('Event: barClick', `[${event.item.title}]`);
  }

  lineClick(event: GanttLineClickEvent) {
      console.log('Event: lineClick', `Source: [${event.source.title}] Target: [${event.target.title}]`);
  }

  dragMoved(event: GanttDragEvent) {}

  dragEnded(event: GanttDragEvent) {
      console.log('Event: dragEnded', `[${event.item.title}]`);
      this.items = [...this.items];
  }

  selectedChange(event: GanttSelectedEvent) {
      event.current && this.ganttComponent.scrollToDate(event.current.start!);

      console.log(
          'Event: selectedChange',
          `Task ids: ${(event.selectedValue as GanttItem[]).map((item) => item.id).join('、')}`
      );
  }

  linkDragEnded(event: GanttLinkDragEvent) {
      this.items = [...this.items];
      console.log('Event: linkDragEnded', `Source: [${event.source.title}] Target: [${event.target!.title}]`);
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
      const sourceItems = event.sourceParent?.children || this.items;
      sourceItems.splice(sourceItems.indexOf(event.source), 1);
      if (event.dropPosition === 'inside') {
          event.target.children = [...(event.target.children || []), event.source];
      } else {
          const targetItems = event.targetParent?.children || this.items;
          if (event.dropPosition === 'before') {
              targetItems.splice(targetItems.indexOf(event.target), 0, event.source);
          } else {
              targetItems.splice(targetItems.indexOf(event.target) + 1, 0, event.source);
          }
      }
      this.items = [...this.items];
  }

  onDragStarted(event: GanttTableDragStartedEvent) {
      console.log('Index drag started', event);
  }

  onDragEnded(event: GanttTableDragEndedEvent) {
      console.log('Index drag ended', event);
  }
}
