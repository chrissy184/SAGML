import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AutomlResultComponent } from './automl-result.component';

describe('AutomlResultComponent', () => {
  let component: AutomlResultComponent;
  let fixture: ComponentFixture<AutomlResultComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AutomlResultComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AutomlResultComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
