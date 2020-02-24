import { Component, OnInit, EventEmitter, Output, ViewChild } from '@angular/core';
import { ApiRoutes, HttpService, UtilService, AlertMessages } from '../../shared';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-repo',
  templateUrl: './repo.component.html',
  styleUrls: ['./repo.component.scss']
})
export class RepoComponent implements OnInit {
  public nyokaRemote: any = {};
  public isContentLoading = false;
  public isLoading = false;
  public listOfRepo: any = [];
  public selectedRepo: any = {};
  public selectedRepoReponse: any = {};
  public filter: any = '';
  public selectedFilter = '';
  constructor(private apiService: HttpService, private utilService: UtilService) { }

  ngOnInit() {
    this.getAllRepo();
  }

  public toggleSidebar(action: string) {
    this.utilService.toggleSidebar(action);
  }

  public getAllRepo(resourceType?: any) {
    this.isLoading = true;
    const options: any = {
      params: {}
    };
    if (resourceType) {
      options.params.ResourceType = resourceType;
    }
    this.apiService.request(ApiRoutes.methods.GET, ApiRoutes.repo, options)
      .pipe(finalize(() => { this.isLoading = false; }))
      .subscribe(response => {
        this.listOfRepo = response;
        // select first record by default
        if (response && response.length) {
          this.getRepoDetails(response[0]);
        }
      });
  }

  public filterRepo() {
    this.getAllRepo(this.selectedFilter)
  }

  public refresh() {
    this.getAllRepo(this.selectedFilter);
  }

  public getRepoDetails(selectedRepo: any) {
    this.selectedRepo = selectedRepo;
    this.selectedRepo.latestVersion = selectedRepo.version;
    this.isContentLoading = true;
    const options = {
      params: {}
    };
    this.apiService.request(ApiRoutes.methods.GET, ApiRoutes.repoGet(selectedRepo.id), options)
      .pipe(finalize(() => { this.isContentLoading = false; }))
      .subscribe(response => {
        this.selectedRepoReponse = response;

        this.filterResponse(selectedRepo.version);
      });
  }

  public filterResponse(version: string) {
    let data = this.selectedRepoReponse;
    if (data && data.items) {
      let items = data.items[0];
      for (let item of items.items) {
        if (item.catalogEntry.version === version) {
          this.selectedRepo = Object.assign(this.selectedRepo, item.catalogEntry);
        }
      }
    }
  }

  public downloadRepo() {
    this.utilService.alert('Work in progress...');
  }

}
