import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FtpPathDetailComponent } from './ftp-path-detail.component';

describe('FTPPathDetailComponent', () => {
  let component: FtpPathDetailComponent;
  let fixture: ComponentFixture<FtpPathDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FtpPathDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FtpPathDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
