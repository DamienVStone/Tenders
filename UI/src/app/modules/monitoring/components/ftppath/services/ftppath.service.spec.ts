import { TestBed } from '@angular/core/testing';

import { FTPPathService } from './ftppath.service';

describe('FTPPathService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: FTPPathService = TestBed.get(FTPPathService);
    expect(service).toBeTruthy();
  });
});
