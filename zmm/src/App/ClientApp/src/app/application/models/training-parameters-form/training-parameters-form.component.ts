import { Component, OnInit, Input, EventEmitter, Output, ViewChild } from '@angular/core';

@Component({
selector: 'app-training-parameters-form',
templateUrl: './training-parameters-form.component.html',
styleUrls: ['./training-parameters-form.component.scss']
})
export class TrainingParametersFormComponent implements OnInit {
@Input() option: any;
@Output() close = new EventEmitter<any>();
@Output() selected = new EventEmitter<any>();
data: any = [];
formData: any = {};
public isLoading = false;
public lossOptions = ['binary_crossentropy', 'categorical_crossentropy', 'MAE', 'RMSE', 'MSE'];
public metricsOptions = ['accuracy', 'f1', 'MAE', 'RMSE', 'MSE'];
public optimizerOptions = ['SGD', 'RMSprop', 'Adagrad', 'Adam', 'Adamax'];
public problemTypeOptions = ['classification', 'regression'];
public scriptOutputOptions = ['NA', 'IMAGE', 'DATA'];

@ViewChild('trainingForm') trainingForm;

constructor() { }

closeForm() {
  this.close.emit(true);
}

submit() {
  this.trainingForm.submitted = true;
  if (this.trainingForm.valid) {
    this.selected.emit(this.formData);
  }
}

ngOnInit() {
  this.formData = {
    batchSize: 15,
    epoch: 100,
    stepPerEpoch: 10,
    learningRate: .001,
    loss: 'categorical_crossentropy',
    metrics: ['accuracy', 'f1'],
    optimizer: 'Adam',
    testSize: .3,
    scriptOutput: 'NA'
  };
}

}
