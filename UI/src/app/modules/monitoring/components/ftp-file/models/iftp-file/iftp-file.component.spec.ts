import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IFtpFileComponent } from './iftp-file.component';

describe('IFtpFileComponent', () => {
  let component: IFtpFileComponent;
  let fixture: ComponentFixture<IFtpFileComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IFtpFileComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IFtpFileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
