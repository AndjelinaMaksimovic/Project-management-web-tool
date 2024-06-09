import { Component, Input, OnChanges, ViewChild } from '@angular/core';
import { Task, TaskService } from '../../../services/task.service';
import { MaterialModule } from '../../../material/material.module';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatMenuTrigger } from '@angular/material/menu';
import { AuthService } from '../../../services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-progress-chip',
  standalone: true,
  imports: [ MaterialModule, ReactiveFormsModule, CommonModule],
  templateUrl: './progress-chip.component.html',
  styleUrl: './progress-chip.component.css'
})
export class ProgressChipComponent implements OnChanges {
  @Input() task: Task | undefined;
  @Input() role: any = {};
  @ViewChild(MatMenuTrigger) trigger!: MatMenuTrigger;
  @ViewChild('inp') inp: any;
  canEdit = false;
  progressView: number = 0  // because task is readonly
  
  progress = new FormControl(0, [Validators.max(100), Validators.min(0), Validators.pattern('^[0-9][0-9]?0?$')])

  constructor(private taskService: TaskService, private authService: AuthService){
    this.progress.markAllAsTouched()
  }
  // async ngOnInit(){
  //   const myId = await this.authService.getMyId()
  //   this.canEdit = this.task?.assignedTo?.some((a: any) => a.id === myId);
  // }
  async ngOnChanges(): Promise<void> {
    if(this.task){
      this.progress.setValue(this.task.progress)
      this.progressView = this.task.progress
      const myId = await this.authService.getMyId()
      this.canEdit = this.task.assignedTo?.some((a: any) => a.id === myId);
    }
  }

  onOpen(){
    this.inp.nativeElement.focus()
    if(this.task)
      // this.progress.setValue(this.task.progress)
      this.progress.setValue(this.progressView)
  }
  selectText(event: any){
    event.target.select()
  }

  async updateProgress(event: any){
    if(!this.task || !this.progress.value){
      this.trigger.closeMenu()
      return;
    }
    if(this.progress.hasError('pattern') || this.progress.hasError('max') || this.progress.hasError('min')){
      event.stopPropagation()
    }else{
      this.trigger.closeMenu()
      await this.taskService.changeTaskProgress(this.task.id, this.progress.value) // TODO: Greska jer api JSON error
      // if(await this.taskService.changeTaskProgress(this.task.id, this.progress.value))
        this.progressView = this.progress.value
    }
  }
}
