import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { LateRegisterationService } from '../services/late-registeration.service';

@Component({
  selector: 'app-late-registeration-page',
  templateUrl: './late-registeration-page.component.html',
  styleUrls: ['./late-registeration-page.component.css']
})
export class LateRegisterationPageComponent implements OnInit {

  constructor(private lateRegisterationService : LateRegisterationService) { }

  ngOnInit(): void {
  }

}
