import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';

@Component({
  selector: 'app-start-page',
  templateUrl: './start-page.component.html',
  styleUrls: ['./start-page.component.css']
})
export class StartPageComponent implements OnInit {


  errorMessage:string = ""
  constructor(private activatedRoute:ActivatedRoute) { }

  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe((params:ParamMap)=>{

      this.errorMessage = params.get('errorMessage')

    })
  }

}
