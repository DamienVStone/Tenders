import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FTPPathListComponent } from './ftppath-list.component';

describe('FTPPathListComponent', () => {
  let component: FTPPathListComponent;
  let fixture: ComponentFixture<FTPPathListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FTPPathListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FTPPathListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
