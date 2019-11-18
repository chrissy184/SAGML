import { Component, OnInit } from '@angular/core';
import { UtilService } from '../../shared';

@Component({
  selector: 'app-repo',
  templateUrl: './repo.component.html',
  styleUrls: ['./repo.component.scss']
})
export class RepoComponent implements OnInit {
 public nyokaRemote: any = {};
  constructor(private utilService: UtilService) { }

  ngOnInit() {
    this.nyokaRemote = this.utilService.getSettingsObject('NR');
    console.log(this.nyokaRemote.url);
  }
  public toggleSidebar(action: string) {
    this.utilService.toggleSidebar(action);
  }

}
