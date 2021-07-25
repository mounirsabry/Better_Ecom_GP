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


  ngOnInit(): void {
  }

  getReadOnlyStatus(){
    this.readOnlyStatusService .getCourseInstanceReadOnlyStatus(this.course_instance_id_get.value).subscribe(
      data =>{
        console.log(data);
      },
      error =>{
        console.log(error.error);
        alert("failes");
      }
    )
  }

}
