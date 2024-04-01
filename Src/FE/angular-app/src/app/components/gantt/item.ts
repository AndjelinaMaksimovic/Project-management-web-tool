export class Item {
    constructor(
        public title: string,
        public start: number,
        public end: number,
        public depends: Items = [],
        public users: Array<any> = [],

        public left = '',
        public width = ''
    ){}
}
export type Items = Array<Item>
export enum GanttColumn {
    title = "title",
    users = "users"
}
export enum TimeScale {
    day = 86_400_000,
    hour = 3_600_000
}