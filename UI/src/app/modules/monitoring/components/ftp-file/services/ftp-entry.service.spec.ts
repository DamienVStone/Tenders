import { TestBed } from '@angular/core/testing';

import { FtpEntryService } from './ftp-entry.service';

describe('FtpEntryService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: FtpEntryService = TestBed.get(FtpEntryService);
    expect(service).toBeTruthy();
  });
});
