import { Component, OnInit } from '@angular/core';
import { ApiRoutes, HttpService, UtilService, AlertMessages } from '../../shared';
import { finalize } from 'rxjs/operators';
import { Router } from '@angular/router';

@Component({
  selector: 'app-models',
  templateUrl: './models.component.html',
  styleUrls: ['./models.component.scss']
})
export class ModelsComponent implements OnInit {

  public listOfModels: any = [];
  public selectedModel: any = {};
  public selectedModelArchitecture: any = {};
  public isLoading = false;
  public uploadFiles = false;
  public showFilterPanel = false;
  public showTrainingFormPanel = false;
  public showCompileFormPanel = false;
  public showCompileError = false;
  public automlFormData: any;
  public isContentLoading = false;
  public filterConfig: any = {};
  public showUploadButton = true;
  message = AlertMessages.MODEL.deleteConfirmationModel;
  public filter: any = '';

  constructor(private apiService: HttpService, private utilService: UtilService, private router: Router) { }

  public dropzoneConfig: any = {
    openFileBrowser: false,
    url: ApiRoutes.models,
    acceptedFiles: `.pmml`,
    acceptedFilesMsg: 'Allowed file type is PMML'
  };
  public tabSelectedIndex = 0;

  public changeSelectedIndex(index: number) {
    this.tabSelectedIndex = index;
  }

  public tabSelectedIndexChanged($event: any) {
  }

  public uploadNewFiles() {
    if (this.tabSelectedIndex === 1) {
      this.dropzoneConfig.openFileBrowser = new Date();
    } else {
      this.dropzoneConfig.openFileBrowser = false;
      this.changeSelectedIndex(1);
      this.uploadFiles = true;
      this.selectedModel = {};
    }
  }

  public uploadFilesDone() {
    this.changeSelectedIndex(0);
    this.getAllPMML();
    this.uploadFiles = false;
  }



  public downloadModel() {
    this.isContentLoading = true;
    const options = {
      responseType: 'blob'
    };
    this.apiService.request(ApiRoutes.methods.GET, ApiRoutes.modelDownload(this.selectedModel.id), options)
      .pipe(finalize(() => { this.isContentLoading = false; }))
      .subscribe(response => {
        this.utilService.saveFile(response, this.selectedModel.name);
        this.utilService.alert(AlertMessages.MODEL.download);
      });

  }

  public getAllPMML(refresh = false) {
    this.isLoading = true;
    const options = {
      params: {
        refresh: refresh
      }
    };
    this.apiService.request(ApiRoutes.methods.GET, ApiRoutes.models, options)
      .pipe(finalize(() => { this.isLoading = false; }))
      .subscribe(responseData => {
        this.listOfModels = responseData;
        // select first record by default
        if (this.listOfModels && this.listOfModels.length && !this.uploadFiles) {
          this.selectPMML(this.listOfModels[0]);
        } else if (!refresh) {
          this.uploadNewFiles();
        }
      }, err => {
        this.uploadNewFiles();
      });
  }

  public refresh() {
    this.getAllPMML(true);
  }

  public selectPMML(selectedModel: any) {
    this.isContentLoading = true;
    this.selectedModel = selectedModel;
    this.changeSelectedIndex(0);
    this.apiService.request(ApiRoutes.methods.GET, ApiRoutes.modelGet(selectedModel.id))
      .pipe(finalize(() => { this.isContentLoading = false; }))
      .subscribe(responseData => {
        this.selectedModel = responseData;
      });
  }

  public updateName(name: any) {
    if (name.innerText === this.selectedModel.id) {
      return;
    }
    this.isLoading = true;
    const options = {
      body: {
        newName: name.innerText
      }
    };
    this.apiService.request(ApiRoutes.methods.PUT, ApiRoutes.modelRename(this.selectedModel.id), options)
      .pipe(finalize(() => { this.isLoading = false; }))
      .subscribe(response => {
        this.selectedModel.id = response.id;
        this.selectedModel.name = response.name;
        this.selectedModel.url = response.url;
        this.selectedModel.filePath = response.filePath;
        console.log(this.selectedModel.id, response.id);
      }, errorResponse => {
        name.innerText = this.selectedModel.id;
      });
  }

  public editpmml() {
    this.isContentLoading = true;
    this.apiService.request(ApiRoutes.methods.GET, ApiRoutes.modelEdit(this.selectedModel.id))
      .pipe(finalize(() => { this.isContentLoading = false; this.changeSelectedIndex(2); }))
      .subscribe(responseData => {
        this.selectedModelArchitecture = responseData;
        this.changeSelectedIndex(2);
      });
  }

  public editpmmlDone() {
    this.changeSelectedIndex(0);
  }

  public deletePMML() {
    this.isLoading = true;
    this.apiService.request(ApiRoutes.methods.DELETE, ApiRoutes.modelGet(this.selectedModel.id))
      .pipe(finalize(() => { this.isLoading = false; }))
      .subscribe(responseData => {
        this.getAllPMML();
        this.utilService.alert(AlertMessages.MODEL.delete);
      });
  }

  public toggleSidebar(action: string) {
    this.utilService.toggleSidebar(action);
  }

  public closeEditor() {
    this.changeSelectedIndex(0);
    this.selectPMML(this.selectedModel);
  }

  public loadInMemory() {
    this.isContentLoading = true;
    const route = this.selectedModel.loaded ? ApiRoutes.modelUnload(this.selectedModel.id) : ApiRoutes.modelLoad(this.selectedModel.id);
    this.apiService.request(ApiRoutes.methods.GET, route)
      .pipe(finalize(() => { this.isContentLoading = false; }))
      .subscribe(responseData => {
        this.selectedModel.loaded = responseData.loaded;
        this.utilService.alert(this.selectedModel.loaded ? AlertMessages.MODEL.load : AlertMessages.MODEL.unload);
      });
  }

  public onCompileFormDataSubmit(body: any) {
    this.isContentLoading = true;
    const options = {
      body: body
    };
    this.apiService.request(ApiRoutes.methods.POST, ApiRoutes.modelCompile(this.selectedModel.id), options)
      .pipe(finalize(() => { this.isContentLoading = false; }))
      .subscribe(responseData => {
        this.utilService.alert(AlertMessages.MODEL.compile);
      }, err => {
        this.showCompileError = true;
        this.utilService.alert(AlertMessages.MODEL.err);

      });
  }

  public compileForm() {
    this.showCompileFormPanel = true;
  }

  public onTrainingFormDataSubmit(body: any) {
    this.isContentLoading = true;
    const options = {
      body: body
    };
    this.apiService.request(ApiRoutes.methods.POST, ApiRoutes.modelTrain(this.selectedModel.id), options)
      .pipe(finalize(() => { this.isContentLoading = false; }))
      .subscribe(responseData => {
        this.utilService.alert(AlertMessages.MODEL.train);
        this.router.navigate(['tasks']);
      });
  }

  public trainPmml() {
    this.showTrainingFormPanel = true;
  }

  public deployModel() {
    this.isContentLoading = true;
    const modelDeployEndPoint = ApiRoutes.modelDeploy(this.selectedModel.id);
    const route = this.selectedModel.deployed ? ApiRoutes.modelDeployUndo(this.selectedModel.id) : modelDeployEndPoint;
    this.apiService.request(ApiRoutes.methods.GET, route)
      .pipe(finalize(() => { this.isContentLoading = false; }))
      .subscribe(responseData => {
        this.selectedModel.deployed = responseData.deployed;
        this.utilService.alert(this.selectedModel.deployed ? AlertMessages.MODEL.deploy : AlertMessages.MODEL.deployUndo);
      });
  }

  public newPmml() {
    this.isLoading = true;
    this.uploadFiles = false;
    this.apiService.request(ApiRoutes.methods.POST, ApiRoutes.modelCreate)
      .pipe(finalize(() => { this.isLoading = false; }))
      .subscribe(responseData => {
        this.utilService.alert(responseData.name + AlertMessages.MODEL.create);
        this.getAllPMML();
      });
  }

  public onModelUploadSuccess($event: any[]) {
    this.listOfModels = $event[1].concat(this.listOfModels);
  }

  public getAutoML(selectedData: any) {
    this.isContentLoading = true;
    this.apiService.request(ApiRoutes.methods.GET, ApiRoutes.dataAutoML(selectedData.id))
      .pipe(finalize(() => { this.isContentLoading = false; }))
      .subscribe(responseData => {
        this.changeSelectedIndex(3);
        this.automlFormData = responseData;
        this.automlFormData.selectedData = selectedData;
      });
  }

  public cancelAutoML() {
    this.changeSelectedIndex(0);
  }

  onFilteredItemSelection($event: any) {
    if ($event && $event.id) {
      this.getAutoML($event);
      this.showFilterPanel = false;
    }
  }

  public autoML() {
    this.showFilterPanel = true;
    this.filterConfig = {
      route: ApiRoutes.data,
      params: {
        type: ['CSV']
      }
    };
  }

  ngOnInit() {
    this.getAllPMML();
  }

}
