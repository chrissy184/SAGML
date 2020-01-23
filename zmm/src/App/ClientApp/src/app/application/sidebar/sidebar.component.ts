import { Component, OnInit } from '@angular/core';
import { UtilService } from '../../shared';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent implements OnInit {
  version: string;
  versionFull: string;
  constructor(private utilService: UtilService) { }

  ngOnInit() {
    this.version = this.utilService.getAppVersion().split('.');
    this.versionFull = this.utilService.getAppVersion();
  }

}
