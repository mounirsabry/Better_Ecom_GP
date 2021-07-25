import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CourseFeedService } from '../services/course-feed.service';

@Component({
  selector: 'app-course-feed',
  templateUrl: './course-feed.component.html',
  styleUrls: ['./course-feed.component.css']
})
export class CourseFeedComponent implements OnInit {

  @Input() public instanceID: string

  courseFeedElements = {}
  logedInType: string = localStorage.getItem('type')
  showEditText: string = 'hidden'
  temp = "ay 7aga"

  courseFeedForm = new FormGroup({
    Content: new FormControl('', Validators.required)
  })
  constructor(private courseFeedService: CourseFeedService) { }

  ngOnInit(): void {
    this.courseFeedService.getInstanceFeed(parseInt(this.instanceID)).subscribe(
      response => {
        this.courseFeedElements = response

      },
      error => {

      }
    )
  }

  get getContent() {
    return this.courseFeedForm.get('Content')
  }

  addToFeed() {

    let obj = this.courseFeedForm.value
    obj['CourseInstanceID'] = parseInt(this.instanceID)

    console.log(obj)
    this.courseFeedService.addToFeed(obj).subscribe(
      response => {
        location.reload()

      },
      error => {
        alert('Failed To Add! instructor maybe not registered in This Course')
      }
    )

  }

  delete(feedID: any) {

    console.log(feedID)
    this.courseFeedService.delete(parseInt(this.instanceID), parseInt(feedID)).subscribe(
      response => {
        location.reload()
      },

      error => {
        alert('Failed To Delete')
      }
    )
  }

}