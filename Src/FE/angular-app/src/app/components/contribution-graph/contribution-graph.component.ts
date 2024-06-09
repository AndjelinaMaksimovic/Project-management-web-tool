import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MaterialModule } from '../../material/material.module';

const DAY = 1000 * 60 * 60 *24;

const RANGE = 200;
const HEIGHT = 7;
function roundTimestamp(timestamp: number){
  const d = new Date(timestamp);
  d.setHours(0, 0, 0, 0);
  return d.getTime();
}
function sameDay(d1: Date, d2: Date) {
  return d1.getFullYear() === d2.getFullYear() &&
    d1.getMonth() === d2.getMonth() &&
    d1.getDate() === d2.getDate();
}
@Component({
  selector: 'app-contribution-graph',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './contribution-graph.component.html',
  styleUrl: './contribution-graph.component.css'
})
export class ContributionGraphComponent {
  @Input() contributions: number[] = [];
  calendarData: {activities: number, date: Date}[] = []
  weeklyData: (typeof this.calendarData)[number][][] = [[]];
  getColor(entry: (typeof this.calendarData)[number]){
    if(entry.activities === 0) return "#EEE";
    if(entry.activities < 10) return "#AAA";
    if(entry.activities < 20) return "#888";
    if(entry.activities < 30) return "#444";
    return "#222";
  }

  ngOnInit(){
    const now = new Date();
    this.calendarData = [];
    for (let d = new Date(Date.now() - DAY * RANGE); d <= now; d.setDate(d.getDate() + 1)) {
        const dayStamp = roundTimestamp(d.getTime());
        this.calendarData.push({
          date: new Date(dayStamp),
          activities: this.contributions.filter((entry: number) => {
            return sameDay(new Date(entry), d);
        }).length || 0,
        });
    }

    const wData = this.calendarData.reduce<(typeof this.calendarData)[number][][]>((acc, e) => {
      if(e.date.getDay() === 1){
        acc.push([]);
      }
      acc.at(-1)!.push(e);
      return acc;
    }, [[]]);
    this.weeklyData = wData;
  }
}
