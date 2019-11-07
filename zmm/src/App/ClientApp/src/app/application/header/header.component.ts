import { Component, OnInit } from '@angular/core';
import { UtilService, HttpService, ApiRoutes } from '../../shared';
import { SwUpdate } from '@angular/service-worker';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
  public userInfo: any = {};
  public isNavOpen = true;
  public title = '';
  public urlCockpit = '';
  public urlDeviceManagment = '';
  constructor(private swUpdate: SwUpdate,
    private utilService: UtilService,
    private apiService: HttpService,
    private router: Router) {
    this.router.events.subscribe(() => {
      console.log(this.router.url.substr(1));
      this.title = this.router.url.substr(1);
    });
  }
  navOpen() {
    this.utilService.toggleSidebar('MENU');
    this.isNavOpen = !this.isNavOpen;
  }
  checkUpdate() {
    if (this.swUpdate.isEnabled) {
      this.swUpdate.available.subscribe((event) => {
        this.utilService.alert('New version available', 'Update', true)
          .onAction().subscribe(() => {
            window.location.reload();
          });
      });
    }
  }
  getUserInfo() {
    this.apiService.request(ApiRoutes.methods.GET, ApiRoutes.accountUserInfo)
      .subscribe(response => {
        this.userInfo = response;
      });
  }
  logout() {
    this.apiService.request(ApiRoutes.methods.GET, ApiRoutes.accountLogout)
      .subscribe(response => {
        this.userInfo = response;
      });
  }
  goToUrl(URL: any) {
    window.location.href = URL;
  }
  getC8YObject() {
    const c8y: any = JSON.parse(localStorage.getItem('settingsJSON'));
    console.log(c8y);
    const c8ySelectedArray = c8y.settings.filter(function (element, index, array) {
      return (element.type === 'C8Y' && element.selected === true);
    });
    return c8ySelectedArray[0];
  }
  ngOnInit() {
    this.checkUpdate();
    this.getUserInfo();
    const c8ySelectedObj: any = this.getC8YObject();
    console.log(c8ySelectedObj);
    if (c8ySelectedObj !== undefined) {
      this.urlCockpit = `${c8ySelectedObj.url}/apps/cockpit`;
      this.urlDeviceManagment = `${c8ySelectedObj.url}/apps/devicemanagement`;
    }
    
  }
}
