import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MaterialModule } from '../../../material/material.module';
import { Task, TaskService } from '../../../services/task.service';
import { SelectComponent } from '../../select/select.component';
import { UserService } from '../../../services/user.service';

@Component({
  selector: 'app-add-user-chip',
  standalone: true,
  imports: [ MaterialModule, SelectComponent ],
  templateUrl: './add-user-chip.component.html',
  styleUrl: './add-user-chip.component.css'
})
export class AddUserChipComponent {
  @Input() task: Task | undefined
  @Output() userOutput: any = new EventEmitter<any>()

  constructor(private userService: UserService, private taskService: TaskService){}

  users: any[] = []
  avaliableUsers: any = []
  userVal: any

  async getUsers(){
    if(!this.task || !this.task.projectId)
      return
    await this.userService.fetchUsersByProject(this.task.projectId)
    this.users = this.task?.assignedTo
    this.avaliableUsers = this.userService.getUsers().filter(allUsers => !this.task?.assignedTo.find((taskUsers: any) => allUsers.id == taskUsers.id))
  }
  // get user(){
  //   return this.userVal
  // }
  // set user(value: any){
  //   this.userVal = value
  //   this.userOutput.emit(value)
  // }
  async addUser(user: any){
    if(!this.task)
      return
    console.log(this.users)

    const tmp = [...this.users]
    tmp.push(user)
    console.log(tmp)
    
    if(await this.taskService.updateTask({id: this.task.id, userIds: tmp.map(usr => usr.id)})){
    // if(await this.taskService.updateTask({id: this.task.id, userId: tmp[0].id})){
      this.users = tmp
      this.userOutput.emit(this.users)
    }
  }
}
