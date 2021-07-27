import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ReadOnlyStatusService } from '../services/read-only-status.service';

@Component({
  selector: 'app-read-only-status',
  templateUrl: './read-only-status.component.html',
  styleUrls: ['./read-only-status.component.css']
})
export class ReadOnlyStatusComponent implements OnInit {

  constructor(private readOnlyStatusService : ReadOnlyStatusService) { }

  getReadStatusForm = new FormGroup({
    course_instance_id : new FormControl('', Validators.required)
  })


  get course_instance_id_get(){
    return this.getReadStatusForm.get('course_instance_id');
  }

  setReadStatusForm = new FormGroup({
    CourseInstanceID : new FormControl('', Validators.required),
    ReadOnlyStatus : new FormControl('', Validators.required)
  })

  get courseInstanceId_get(){
    return this.setReadStatusForm.get('CourseInstanceID');
  }

  get read_only_status_get(){
    return this.setReadStatusForm.get('ReadOnlyStatus');
  }

  status : boolean
  hide : boolean = true
  ngOnInit(): void {
  }

  getReadOnlyStatus(){
    this.readOnlyStatusService .getCourseInstanceReadOnlyStatus(this.course_instance_id_get.value).subscribe(
      data =>{
        console.log(data);
        this.status = data;
        this.hide = false;
      },
      error =>{
        console.log(error.error);
        alert("failed");
      }
    )
    return this.status;
  }

  setCourseInstanceStatus(){
    var obj = {
      'CourseInstanceID' : this.courseInstanceId_get.value,
      'ReadOnlyStatus' : (this.read_only_status_get.value === '1')
    }
    console.log(obj);
    this.readOnlyStatusService.setCourseInstanceReadOnlyStatus(obj).subscribe(
      data =>{
        console.log(data);
        alert("Status Changed");
      },
      error =>{
        console.log(error.error);
        alert("failed, instructor maybe not registered in the course");
      }
    )
  }

}
