import { Component, OnInit } from '@angular/core';
import { UtilService, HttpService, ApiRoutes, AlertMessages } from '../../shared';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {
  public isLoading = false;
  public isContentLoading = false;
  public editorOptions: any = {
    theme: 'vs-light',
    language: 'json'
  };

  public defaultSettingsJSON: any = `{
    "settings": [{
      "name": "Cumulocity",
      "type": "C8Y",
      "tenantID": "ai",
      "username": "*******",
      "password": "*******",
      "url": "https://ai.eu-latest.cumulocity.com",
      "selected": true
    },
    {
      "name": "Cumulocity",
      "type": "C8Y",
      "tenantID": "ai",
      "username": "*******",
      "password": "*******",
      "url": "https://ai.cumulocity.com",
      "selected": false
    },
    {
      "name": "Predictive Analytics",
      "type": "ZS",
      "tenantID": "ai",
      "username": "*******",
      "password": "*******",
      "url": "https://ai.eu-latest.cumulocity.com",
      "selected": true
    },
    {
      "name": "Repo Server",
      "type": "NR",
      "tenantID": "repo",
      "username": "*******",
      "password": "*******",
      "url": "https://repo.umoya.ai/",
      "selected": true
    },
    {
      "name": "Repo Server",
      "type": "NR",
      "tenantID": "hub",
      "username": "*******",
      "password": "*******",
      "url": "https://hub.umoya.ai/",
      "selected": true
    }
  ]
}
  `;
  public settingsJSON: any = `{}`;
  constructor(private utilService: UtilService, private apiService: HttpService) { }

  public getSettings() {
    this.isContentLoading = true;
    this.apiService.request(ApiRoutes.methods.GET, ApiRoutes.settings)
      .pipe(finalize(() => { this.isContentLoading = false; }))
      .subscribe(response => {
        this.settingsJSON = JSON.stringify(response);
        localStorage.setItem('settingsJSON', this.settingsJSON);
      });
  }
  ngOnInit() {
    this.getSettings();
  }
  public toggleSidebar(action: string) {
    this.utilService.toggleSidebar(action);
  }
  public saveSettings() {
    this.isContentLoading = true;
    const options = {
      body: this.settingsJSON
    };
    this.apiService.request(ApiRoutes.methods.POST, ApiRoutes.settings, options)
      .pipe(finalize(() => { this.isContentLoading = false; }))
      .subscribe(response => {
        this.settingsJSON = JSON.stringify(response);
        localStorage.setItem('settingsJSON', this.settingsJSON);
        this.utilService.alert('Settings Saved.');
      });
  }
}
