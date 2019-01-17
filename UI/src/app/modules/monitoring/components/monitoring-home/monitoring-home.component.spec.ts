import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MonitoringHomeComponent } from './monitoring-home.component';

describe('MonitoringHomeComponent', () => {
  let component: MonitoringHomeComponent;
  let fixture: ComponentFixture<MonitoringHomeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MonitoringHomeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MonitoringHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
