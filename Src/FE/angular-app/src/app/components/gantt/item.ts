export class Item {
    constructor(
        // public title: string,
        // public start: number,
        // public end: number,
        // public depends: Items = [],
        // public users: Array<User> = [],

        public id: number,
        public title: string,
        public description: string,
        public category: string,
        public priority: 'Low' | 'Medium' | 'High',
        public status: string,
        public startDate: number = Date.now(),
        public dueDate: number,
        public assignedTo: User[] = [],
        public dependant: number[] = [], // ID-s of tasks that depend on this one

        public color = '#5096A4',
        public left = 0,
        public width = 0,
        public display = true,
        public hover = false
    ){}
}
export enum GanttColumn {
    tasks = "Tasks",
    users = "Users"
}
export enum TimeScale {
    week = 86_400_000 * 7,
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

export class Milestone{
    constructor(
        public id: number,
        public title: string,
        public date: number,
        public assignedTo: User[] = [],
        public dependant: number[] = [], // ID-s of tasks that depend on this one
        
        public color = '#5096A4',
        public left = 0,
        public width = 0,
        public display = true,
        public hover = false
    ){}
}