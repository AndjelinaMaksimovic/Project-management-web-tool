import { TestBed } from '@angular/core/testing';

import { PriorityService } from './priority.service';

describe('PriorityService', () => {
  let service: ProjectService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PriorityService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
