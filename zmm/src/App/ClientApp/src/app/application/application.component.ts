import { Component, OnInit, ViewChild } from '@angular/core';
import { UtilService } from '../shared';
import { MatSidenav } from '@angular/material';

@Component({
  selector: 'app-application',
  templateUrl: './application.component.html',
  styleUrls: ['./application.component.scss']
})
export class ApplicationComponent implements OnInit {

  public sidenavOptions = {
    bottom: 0,
    fixed: false,
    top: 0
  };
  public onlineAlert: any;
  public defaultSettingsJSON: any = `{
    "settings": [{
      "name": "Cumulocity",
      "type": "C8Y",
      "tenantID": "ai",
      "username": "vran",
      "password": "Testing@123",
      "url": "https://ai.eu-latest.cumulocity.com",
      "selected": true
    },
    {
      "name": "Cumulocity",
      "type": "C8Y",
      "tenantID": "ai",
      "username": "vran",
      "password": "Testing@123",
      "url": "https://ai.cumulocity.com",
      "selected": false
    },
    {
      "name": "Zementis Server",
      "type": "ZS",
      "tenantID": "ai",
      "username": "vran",
      "password": "Testing@123",
      "url": "https://ai.eu-latest.cumulocity.com",
      "selected": true
    },
    {
      "name": "Zementis Server",
      "type": "ZS",
      "tenantID": "zserver",
      "username": "",
      "password": "",
      "url": "https://zserver.zmod.org/adapars/",
      "selected": false
    },
    {
      "name": "Nyoka Remote",
      "type": "NR",
      "tenantID": "dlexp",
      "username": "",
      "password": "",
      "url": "https://dlexp.zmod.org/",
      "selected": true
    }
  ]
}
  `;
  @ViewChild('sidenav') public sidenav: MatSidenav;

  constructor(private utilService: UtilService) { }

  ngOnInit() {

    const settingsJSON = localStorage.getItem('settingsJSON');
    if (!settingsJSON) {
      localStorage.setItem('settingsJSON', this.defaultSettingsJSON);
    }

    this.utilService.sidebarAction.subscribe((action) => {
      if (action === 'MENU') {
        this.sidenav.toggle();
      }
      if (action === 'FULLSCREEN') {
        this.sidenav.open();
      }
      if (action === 'FULLSCREEN_EXIT') {
        this.sidenav.close();
      }
    });
    this.utilService.checkInternetConnection().subscribe(async (online) => {
      const isOnline = await online;
      if (isOnline && this.onlineAlert) {
        this.onlineAlert.dismiss();
      } else if (!isOnline) {
        this.onlineAlert = this.utilService.alert('You are offline. Some functionality may be unavailable.', '', true);
      }
    });
  }
}
