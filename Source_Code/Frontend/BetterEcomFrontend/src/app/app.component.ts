import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'BetterEcomFrontend';

  currentUrl = window.location.href


  constructor(private location:Location){

  }
  ngOnInit(){

  }

  getCurrentUrl(url:string):boolean{
    // reload is important as to get the correct current url not the previous.
    window.location.reload
    this.currentUrl = window.location.href

    return !this.currentUrl.includes(url)

  }
}
