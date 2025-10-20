import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MobileNavMenu } from './mobile-nav-menu';
import {provideRouter} from '@angular/router';

describe('MobileNavMenu', () => {
  let component: MobileNavMenu;
  let fixture: ComponentFixture<MobileNavMenu>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MobileNavMenu],
      providers: [provideRouter([])],
    })
    .compileComponents();

    fixture = TestBed.createComponent(MobileNavMenu);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
