export class Item {
    constructor(
        public title: string,
        public start: number,
        public end: number,
        public depends: Items = [],
        public users: Array<any> = [],

        public color = '#5096A4',
        public left = '',
        public width = '',
        public display = true
    ){}
}
export type Items = Array<Item>
export enum GanttColumn {
    tasks = "Tasks",
    users = "Users"
}
export enum TimeScale {
    day = 86_400_000,
    hour = 3_600_000
}