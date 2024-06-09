import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

const DAY = 1000 * 60 * 60 *24;

const RANGE = 150;
const HEIGHT = 7;
function roundTimestamp(timestamp: number){
  const d = new Date(timestamp);
  d.setHours(0, 0, 0, 0);
  return d.getTime();
}
@Component({
  selector: 'app-contribution-graph',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './contribution-graph.component.html',
  styleUrl: './contribution-graph.component.css'
})
export class ContributionGraphComponent {
  @Input() contributions: number[] = [];
  calendarData: {activities: number, date: Date}[] = []

  public get weeklyData(){
    const wData = this.calendarData.reduce<(typeof this.calendarData)[number][][]>((acc, e) => {
      if(e.date.getDay() === 1){
        acc.push([]);
      }
      acc.at(-1)!.push(e);
      return acc;
    }, [[]]);
    return wData;
  }

  constructor(){
    const contributionMap = this.contributions.reduce<Record<string, number>>((acc, e) => {
      const timestamp = roundTimestamp(e);
      if(!acc[timestamp]) acc[timestamp] = 0;;
      acc[timestamp] = acc[timestamp] + 1;
      return acc;
    }, {});
    const now = new Date();
    this.calendarData = [];
    for (let d = new Date(Date.now() - DAY * RANGE); d <= now; d.setDate(d.getDate() + 1)) {
        const dayStamp = d.getTime();
        this.calendarData.push({
          date: new Date(dayStamp),
          activities: contributionMap[dayStamp],
        });
    }
  }
}
