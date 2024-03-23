import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import GSTC, { Config, GSTCResult, Items, Rows } from 'gantt-schedule-timeline-calendar';
import { Plugin as TimelinePointer } from 'gantt-schedule-timeline-calendar/dist/plugins/timeline-pointer.esm.min.js';
import { Plugin as Selection } from 'gantt-schedule-timeline-calendar/dist/plugins/selection.esm.min.js';

declare global {
  var state: any;
  var gstc: any;
}

@Component({
  selector: 'app-gantt-test',
  standalone: true,
  imports: [],
  templateUrl: './gantt-test.component.html',
  styleUrl: './gantt-test.component.css'
})
export class GanttTestComponent implements OnInit {
  
  @ViewChild('gstcElement', { static: true }) gstcElement?: ElementRef;
  gstc?: GSTCResult;

  generateConfig(): Config {
    const iterations = 400;
    // GENERATE SOME ROWS

    const rows: Rows = {};
    for (let i = 0; i < iterations; i++) {
      const withParent = i > 0 && i % 2 === 0;
      const id = GSTC.api.GSTCID(i.toString());
      rows[id] = {
        id,
        label: 'Room ' + i,
        parentId: withParent ? GSTC.api.GSTCID((i - 1).toString()) : undefined,
        expanded: false,
      };
    }

    // GENERATE SOME ROW -> ITEMS

    let start = GSTC.api.date().startOf('day').subtract(30, 'day');
    const items: Items = {};
    for (let i = 0; i < iterations; i++) {
      const id = GSTC.api.GSTCID(i.toString());
      start = start.add(1, 'day');
      items[id] = {
        id,
        label: 'User id ' + i,
        time: {
          start: start.valueOf(),
          end: start.add(1, 'day').valueOf(),
        },
        rowId: id,
      };
    }

    // LEFT SIDE LIST COLUMNS

    const columns = {
      percent: 100,
      resizer: {
        inRealTime: true,
      },
      data: {
        [GSTC.api.GSTCID('label')]: {
          id: GSTC.api.GSTCID('label'),
          data: 'label',
          expander: true,
          isHtml: true,
          width: 230,
          minWidth: 100,
          header: {
            content: 'Room',
          },
        },
      },
    };

    return {
      licenseKey:
        '====BEGIN LICENSE KEY====\nge9h6u+Jgtb26b4MhwW3hEdzyzLGSGBf5KlZgGzXJDGqZuYrn00Nld/Mu0JyRYlgE+XVs+xK/OBd5LIyTgPvqlsip4Avk8CfCUwAmwEbSvpI5egOHpicGtLNR2ZgkgCfLSpBV+q4I2jn72l9cejLFlJpSKfX6R22t0n3gzJmMH5Z5hCLudd1pd+RA9eJnJeKZC6qqc3X4AtE3ai5k8aoMfNveNEqRRj+iC9oaP7bDiZtLsaIk8oAoqgYnSk3jX7lgpzS7Pp5lguDsXm+rw551Nk2VFNvn54EKky5hzlVLyNKrgFaDl/Kna8wRbPhsI9Xf+m4kqQgQax7/ZyHiauYKQ==||U2FsdGVkX18+zO9+12G1CFIl2whXLnWIt8ofIKiqMGPY1lbSPt6UY78UNzwaZQirVB8ksEw5zKsPLYpybJ/8IDqvw6z0wME7LLhAwBo3zIg=\nG/IeDOC949Rnik7w/QFugK+eGZwHxaBEZEvF3O9AqwmRSCNjbTixuDoEXM2qUCIOUkPxezhi8DggkauTRzkPc96G2Rcosxldd6tIWatMy47ri/1Jd3v0cvcn18Sbr31R5BpxOfg/i/gBoSxTPGgmTWq5QqVh3b63OscFboJfasgv3xWJ1UhBfWNv3tK55OKt14FmJjVhkbRP+vngHjcmx8iA1ETNOqsoUay/8RnzA9xI36xJ770U/JEEyPKSCOZVPdJWHgF46620pmPzj78nvF+gejsKOG/MUGkKolkeXhSV+I97z+GYZu18IZhH0mYFGtuDOHns6PzowQJtTao3Fg==\n====END LICENSE KEY====',
      list: {
        rows,
        columns,
      },
      chart: {
        items,
      },
      plugins: [TimelinePointer(), Selection()],
    };
  }

  ngOnInit(): void {
    const state = GSTC.api.stateFromConfig(this.generateConfig());
    globalThis.state = state;
    this.gstc = GSTC({
      element: this.gstcElement?.nativeElement,
      state,
    });
    globalThis.gstc = this.gstc;
  }

  updateFirstItem(): void {
    this.gstc?.state.update(
      `config.chart.items.${GSTC.api.GSTCID('0')}`,
      (item: any) => {
        item.label = 'Dynamically updated!';
        return item;
      }
    );
  }

  updateFirstRow(): void {
    this.gstc?.state.update(
      `config.list.rows.${GSTC.api.GSTCID('0')}`,
      (row: any) => {
        row.label = 'Dynamically updated!';
        return row;
      }
    );
  }

  scrollToCurrentTime(): void {
    this.gstc?.api.scrollToTime(GSTC.api.date().valueOf());
  }

  clearSelection(): void {
    this.gstc?.api.plugins.Selection.selectCells([]);
    this.gstc?.api.plugins.Selection.selectItems([]);
  }
}
