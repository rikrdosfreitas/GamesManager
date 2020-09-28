import { Component, OnInit, OnDestroy } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
@Component({
  selector: 'app-auto-login',
  templateUrl: './auto-login.component.html',
  styleUrls: ['./auto-login.component.scss']
})
export class AutoLoginComponent implements OnInit {

  constructor(public oidcSecurityService: OidcSecurityService) { }

  ngOnInit() {

    console.log("loggin");
    this.oidcSecurityService.authorize();
  }
}