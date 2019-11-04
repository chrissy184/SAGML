import { Component, OnInit } from '@angular/core';
import { UtilService } from '../../shared';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {
  public isLoading = false;
  public editorOptions: any = {
    theme: 'vs-light',
    language: 'json'
  };
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
  public settingsJSON: any = `{}`;
  constructor(private utilService: UtilService) { }

  ngOnInit() {
    this.settingsJSON = localStorage.getItem('settingsJSON');
    if (!this.settingsJSON) {
      this.settingsJSON = this.defaultSettingsJSON;
    }
  }
  public toggleSidebar(action: string) {
    this.utilService.toggleSidebar(action);
  }
  public saveSettings() {
    localStorage.setItem('settingsJSON', this.settingsJSON);
    this.utilService.alert('Settings Saved.');
  }

}
