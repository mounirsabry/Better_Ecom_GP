import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { GeneralFeedService } from '../services/general-feed.service';

@Component({
  selector: 'app-general-feed',
  templateUrl: './general-feed.component.html',
  styleUrls: ['./general-feed.component.css']
})
export class GeneralFeedComponent implements OnInit {

  @Input() public type:string
  constructor(private generalFeedService:GeneralFeedService) { }

  currentFeedId:number
  showEditText:string = 'hidden';
  generalFeedElements = []

  feedBoxForm = new FormGroup({
    Content: new FormControl('',Validators.required)
  })
  ngOnInit(): void {


    this.generalFeedService.getGeneralFeed().subscribe(
      response =>{
        this.generalFeedElements = response;

        /*for(let key of Object.keys(response)){

          if(key == 'insertion_date')
            this.generalFeedElements[key] = new Date(response[key])
          else
            this.generalFeedElements[key] = response[key]
        }*/
      },
      error =>{

      }
    )
  }

  addToGeneralFeed(){

    this.generalFeedService.addToGeneralFeed(this.feedBoxForm.value).subscribe(
      response => {
        // to make the new feed show

       //alert('addition successfull!')
        location.reload()


      },
      error =>{
        alert('Failed to Add!')
      }
    )

  }

  delete(ID){

    this.generalFeedService.delete(parseInt(ID)).subscribe(
      response =>{

        location.reload()
      },
      error =>{

      }
    )
  }
  get getContent(){
    return this.feedBoxForm.get('Content')
  }

  setCurrentFeedId(id:number){
    this.currentFeedId = id
  }

}
