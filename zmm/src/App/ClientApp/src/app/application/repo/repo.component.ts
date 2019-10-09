import { Component, OnInit } from '@angular/core';
import { UtilService } from '../../shared';

@Component({
  selector: 'app-repo',
  templateUrl: './repo.component.html',
  styleUrls: ['./repo.component.scss']
})
export class RepoComponent implements OnInit {

  public listOfRepoData = [{ "loaded": false, "deployed": false, "modelName": null, "dateCreated": "2019-06-26T09:25:23", "id": "cancer_svc", "name": "cancer_svc.pmml", "user": "", "created_on": "6/26/19 9:25:23 AM", "edited_on": "6/26/19 9:25:23 AM", "type": "PMML", "url": "/model/cancer_svc.pmml", "filePath": "/home/ubuntu/Repos/ZMOD/Models/cancer_svc.pmml", "size": 137519, "mimeType": "application/octet-stream", "extension": "pmml", "properties": [] }, { "loaded": false, "deployed": false, "modelName": null, "dateCreated": "2019-06-26T04:51:03", "id": "adms", "name": "adms.pmml", "user": "", "created_on": "6/26/19 4:51:03 AM", "edited_on": "6/26/19 4:51:03 AM", "type": "PMML", "url": "/model/adms.pmml", "filePath": "/home/ubuntu/Repos/ZMOD/Models/adms.pmml", "size": 2638, "mimeType": "application/octet-stream", "extension": "pmml", "properties": [] }, { "loaded": false, "deployed": false, "modelName": null, "dateCreated": "2019-06-26T04:44:59", "id": "New_636971210439769208", "name": "New_636971210439769208.pmml", "user": "", "created_on": "6/26/19 4:44:59 AM", "edited_on": "6/26/19 4:44:59 AM", "type": "PMML", "url": "/model/New_636971210439769208.pmml", "filePath": "/home/ubuntu/Repos/ZMOD/Models/New_636971210439769208.pmml", "size": 22794912, "mimeType": "application/octet-stream", "extension": "pmml", "properties": [] }, { "loaded": false, "deployed": false, "modelName": null, "dateCreated": "2019-06-26T04:41:51", "id": "Distracted_Driver_PP", "name": "Distracted_Driver_PP.pmml", "user": "", "created_on": "6/26/19 4:41:51 AM", "edited_on": "6/26/19 4:41:51 AM", "type": "PMML", "url": "/model/Distracted_Driver_PP.pmml", "filePath": "/home/ubuntu/Repos/ZMOD/Models/Distracted_Driver_PP.pmml", "size": 22785366, "mimeType": "application/octet-stream", "extension": "pmml", "properties": [] }, { "dateCreated": "2019-07-01T09:31:30.9773017+00:00", "id": "Predicted_admissions", "name": "Predicted_admissions.csv", "user": "", "created_on": "7/1/19 9:31:30 AM", "edited_on": "7/1/19 9:31:30 AM", "type": "CSV", "url": "/api/data/preview/Predicted_admissions.csv", "filePath": "/home/ubuntu/Repos/ZMOD/Data/Predicted_admissions.csv", "size": 20518, "mimeType": "application/json", "extension": "csv", "properties": [] }, { "dateCreated": "2019-06-26T04:45:16.5572998+00:00", "id": "New_636971210439769208", "name": "New_636971210439769208", "user": null, "created_on": "6/26/19 4:45:16 AM", "edited_on": "6/26/19 4:45:16 AM", "type": "FOLDER", "url": null, "filePath": "/home/ubuntu/Repos/ZMOD/Data/New_636971210439769208", "size": 0, "mimeType": "", "extension": "", "properties": [{ "key": "Subdirectories", "value": "0" }, { "key": "Files", "value": "0" }] }, { "dateCreated": "2019-06-26T04:43:20.7856626+00:00", "id": "admissions_test", "name": "admissions_test.csv", "user": "", "created_on": "6/26/19 4:43:20 AM", "edited_on": "6/26/19 4:43:20 AM", "type": "CSV", "url": "/api/data/preview/admissions_test.csv", "filePath": "/home/ubuntu/Repos/ZMOD/Data/admissions_test.csv", "size": 15897, "mimeType": "text/csv", "extension": "csv", "properties": [{ "key": "Number of Rows", "value": "645" }, { "key": "Number of Columns", "value": "2" }] }, { "dateCreated": "2019-06-26T04:43:20.7336627+00:00", "id": "admissions", "name": "admissions.csv", "user": "", "created_on": "6/26/19 4:43:20 AM", "edited_on": "6/26/19 4:43:20 AM", "type": "CSV", "url": "/api/data/preview/admissions.csv", "filePath": "/home/ubuntu/Repos/ZMOD/Data/admissions.csv", "size": 17192, "mimeType": "text/csv", "extension": "csv", "properties": [{ "key": "Number of Rows", "value": "645" }, { "key": "Number of Columns", "value": "3" }] }, { "dateCreated": "2019-06-26T04:42:10.7898819+00:00", "id": "img_100004", "name": "img_100004.jpg", "user": "", "created_on": "6/26/19 4:42:10 AM", "edited_on": "6/26/19 4:42:10 AM", "type": "IMAGE", "url": "/api/data/preview/img_100004.jpg", "filePath": "/home/ubuntu/Repos/ZMOD/Data/img_100004.jpg", "size": 37891, "mimeType": "image/jpg", "extension": "jpg", "properties": [{ "key": "Width", "value": "640 px" }, { "key": "Height", "value": "480 px" }] }, { "dateCreated": "2019-06-26T04:42:10.7898819+00:00", "id": "img_100009", "name": "img_100009.jpg", "user": "", "created_on": "6/26/19 4:42:10 AM", "edited_on": "6/26/19 4:42:10 AM", "type": "IMAGE", "url": "/api/data/preview/img_100009.jpg", "filePath": "/home/ubuntu/Repos/ZMOD/Data/img_100009.jpg", "size": 42355, "mimeType": "image/jpg", "extension": "jpg", "properties": [{ "key": "Width", "value": "640 px" }, { "key": "Height", "value": "480 px" }] }, { "dateCreated": "2019-06-26T04:41:32.3300024+00:00", "id": "DisDriver", "name": "DisDriver", "user": null, "created_on": "6/26/19 4:41:32 AM", "edited_on": "6/26/19 4:41:32 AM", "type": "FOLDER", "url": null, "filePath": "/home/ubuntu/Repos/ZMOD/Data/DisDriver", "size": 0, "mimeType": "", "extension": "", "properties": [{ "key": "Subdirectories", "value": "22" }, { "key": "Files", "value": "3324" }] }];
  public filter: any = '';
  public selectedData: any = {};
  public showFilterPanel = false;
  public isLoading = false;
  public isContentLoading = false;
  public devicesListData: any = [
    {
      name: 'R11'
    },
    {
      name: 'R12'
    },
    {
      name: 'R13'
    },
    {
      name: 'R14'
    }
  ];
  public tabSelectedIndex = 0;
  constructor(private utilService: UtilService) { }

  ngOnInit() {
    this.selectedData = this.listOfRepoData[0];
  }

  viewData(item: any) {
    this.selectedData = item;
    console.log(this.selectedData);
  }
  public toggleSidebar(action: string) {
    this.utilService.toggleSidebar(action);
  }
  public displayDevies() {
    this.showFilterPanel = true;
  }
  public closeFilter() {
    this.showFilterPanel = false;
  }

}
