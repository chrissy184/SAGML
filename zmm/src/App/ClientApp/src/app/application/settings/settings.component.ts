import { Component, OnInit } from '@angular/core';
import { UtilService } from '../../shared';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {

  public filter: any = '';
  public listOfSettings: any = [];
  public selectedSetting: any = {};
  public isLoading = false;
  constructor(private utilService: UtilService) { }

  ngOnInit() {
    this.listOfSettings = [
      {
        id: 'Cumulocity',
        name: 'Cumulocity',
        properties: [
          {
            key: 'Tanent ID',
            value: 'ai'
          },
          {
            key: 'User ID',
            value: 'testuser'
          },
          {
            key: 'Password',
            value: 'xxxxxx'
          },
          {
            key: 'Url',
            value: 'https://ai.cumulocity.com/apps/cockpit/index.html'
          }
        ]
      },
      {
        id: 'Nyoka Remote',
        name: 'Nyoka Remote',
        properties: [
          {
            key: 'Tanent ID',
            value: 'dlexp'
          },
          {
            key: 'User ID',
            value: 'testuser'
          },
          {
            key: 'Password',
            value: 'xxxxxx'
          },
          {
            key: 'Url',
            value: 'https://dlexp.zmod.org/'
          }
        ]
      },
      {
        id: 'Zementis Server',
        name: 'Zementis Server',
        properties: [
          {
            key: 'Tanent ID',
            value: 'zserver'
          },
          {
            key: 'User ID',
            value: 'testuser'
          },
          {
            key: 'Password',
            value: 'xxxxxx'
          },
          {
            key: 'Url',
            value: 'https://zserver.zmod.org/adapars/'
          }
        ]
      }
    ];
    this.selectedSetting = this.listOfSettings[0];
  }
  public toggleSidebar(action: string) {
    this.utilService.toggleSidebar(action);
  }
  public selectSetting(item: any) {
    this.selectedSetting = item;
  }

}
