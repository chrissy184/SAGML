import { Component, OnInit, OnChanges, ViewChild, Input } from "@angular/core";
import {
  ApiRoutes,
  HttpService,
  UtilService,
  AlertMessages
} from "../../../shared";
import { finalize } from "rxjs/operators";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { MatTableDataSource } from "@angular/material/table";
import {
  animate,
  state,
  style,
  transition,
  trigger
} from "@angular/animations";
@Component({
  selector: "app-automl-result",
  templateUrl: "./automl-result.component.html",
  styleUrls: ["./automl-result.component.scss"],
  animations: [
    trigger("detailExpand", [
      state(
        "collapsed",
        style({ height: "0px", minHeight: "0", display: "none" })
      ),
      state("expanded", style({ height: "*" })),
      transition(
        "expanded <=> collapsed",
        animate("225ms cubic-bezier(0.4, 0.0, 0.2, 1)")
      )
    ])
  ]
})
export class AutomlResultComponent implements OnInit, OnChanges {
  public isLoading = false;
  public isContentLoading = false;
  public displayedColumnsGenerationResults: string[] = [
    "expand",
    "modelName",
    "score",
    "bestmodel"
  ];
  @Input() response: any = {};
  public selectedAutoMLHistory: any = {};
  @Input() selectedHistory: any;
  public dataSourceTaskHistoryGenerationResults: any = [];
  @ViewChild("generationResultsMatPaginator") paginatorGenerationResults: MatPaginator;
  @ViewChild("generationResultsMatSort", { static: true }) sortGenerationResults: MatSort;
  constructor(private apiService: HttpService, private utilService: UtilService) { }

  ngOnChanges() {
    if (this.response) {
      this.dataSourceTaskHistoryGenerationResults = new MatTableDataSource(this.response.generationInfo);
    } else {
      this.dataSourceTaskHistoryGenerationResults = new MatTableDataSource([]);
    }
    this.dataSourceTaskHistoryGenerationResults.paginator = this.paginatorGenerationResults;
    this.dataSourceTaskHistoryGenerationResults.sort = this.sortGenerationResults;
  }
  ngOnInit(): void {

  }

  public selectTaskHistory(selectedAutoMLHistory: any, expandedElement: any) {
    console.log(selectedAutoMLHistory, expandedElement);
    if (expandedElement) {
      this.selectedAutoMLHistory = selectedAutoMLHistory;
    }
  }
  public saveModel() {
    console.log(this.selectedHistory);
    const options = {
      body: {
        filePath: this.selectedHistory.pmmlFilelocation,
        fileName: this.selectedHistory.newPMMLFileName
      }
    };
    this.isContentLoading = true;
    this.apiService.request(ApiRoutes.methods.POST, ApiRoutes.taskSaveModel(this.selectedHistory.idforData), options)
      .pipe(finalize(() => { this.isContentLoading = false; }))
      .subscribe(() => {
        this.utilService.alert(this.selectedHistory.newPMMLFileName + AlertMessages.TASK.save);
      });
  }
}
