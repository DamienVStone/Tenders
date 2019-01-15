import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-ftppath-detail',
  templateUrl: './ftppath-detail.component.html',
  styleUrls: ['./ftppath-detail.component.scss']
})
export class FTPPathDetailComponent implements OnInit {

  constructor() { }

  ngOnInit() {
    console.log('Details inited');
  }

}
