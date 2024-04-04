export class Item {
    constructor(
        // public title: string,
        // public start: number,
        // public end: number,
        // public depends: Items = [],
        // public users: Array<User> = [],
        public name: string,
        public description: string,
        public categoryId: number,
        public priorityId: number,
        public statusId: number,
        public startDate: number = Date.now(),
        public dueDate: number,
        // public depends: Items = [],
        public assignedTo: Array<User> = [],

        public color = '#5096A4',
        public left = '',
        public width = '',
        public display = true,
        public hover = false
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
export class User{
    constructor(
        public id: number,
        public firstName: string = '',
        public lastName: string = '',
        public profilePicture: any = ''
    ){}
}