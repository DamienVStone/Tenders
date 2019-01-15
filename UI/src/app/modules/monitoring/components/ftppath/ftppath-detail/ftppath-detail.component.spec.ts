import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FTPPathDetailComponent } from './ftppath-detail.component';

describe('FTPPathDetailComponent', () => {
  let component: FTPPathDetailComponent;
  let fixture: ComponentFixture<FTPPathDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FTPPathDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FTPPathDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
