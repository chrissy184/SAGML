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
  goToUrl(URL: any){
    window.location.href = URL;
  }
  ngOnInit() {
    this.checkUpdate();
    this.getUserInfo();
  }
}
