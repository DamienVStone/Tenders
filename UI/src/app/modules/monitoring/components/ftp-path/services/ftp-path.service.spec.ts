import { TestBed } from '@angular/core/testing';

import { FtpPathService } from './ftp-path.service';

describe('FtpPathService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: FtpPathService = TestBed.get(FtpPathService);
    expect(service).toBeTruthy();
  });
});
