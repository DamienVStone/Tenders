import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FtpEntryListComponent } from './ftp-entry-list.component';

describe('FtpEntryListComponent', () => {
  let component: FtpEntryListComponent;
  let fixture: ComponentFixture<FtpEntryListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FtpEntryListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FtpEntryListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
