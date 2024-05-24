import { Column } from "./item";

export class helpers{
    static range(start: number, end: number, step: number = 1){ // inclusive
        return Array(Math.floor((end - start) / step + 1)).fill(0).map((_, i) => start + i * step)
    }
    static colOffset(idx: number, columns: Column[]){
        const colWidths = columns.map(col => col.width)
        return colWidths.slice(0, idx+1).reduce((a,b)=>a+b,0)
    }

    static includeDay(day: number, hideWeekend: boolean, holidays: Date[]){
        const d = new Date(day);
        d.setHours(0, 0, 0, 0)
        day = d.getDay();
        if (hideWeekend && (day == 0 || day == 6))
            return false
        if(holidays.includes(d))
            return false
        return true
    }
}