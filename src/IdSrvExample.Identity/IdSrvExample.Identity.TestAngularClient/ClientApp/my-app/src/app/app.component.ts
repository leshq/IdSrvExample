import { Component, OnDestroy, OnInit } from '@angular/core';
import { OidcClientNotification, OidcSecurityService, PublicConfiguration } from 'angular-auth-oidc-client';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  constructor(
    public oidcSecurityService: OidcSecurityService,
    public http: HttpClient
    ) 
    { }

  ngOnInit() {
    this.oidcSecurityService
    .checkAuth()
    .subscribe((auth) => console.log('is authenticated', auth));
  }

  login() {
    this.oidcSecurityService.authorize();
  }

  logout() {
    this.oidcSecurityService.logoff();
  }

  callApi(){
    var options = this.getHttpOptions();

    this.http.get("https://localhost:5010/authtest/private", options)
      .subscribe((data: any) => {
        console.log("api result:", data);
      });
  }

  callApiAdmin(){
    var options = this.getHttpOptions();

    this.http.get("https://localhost:5010/authtest/admin", options)
      .subscribe((data: any) => {
        console.log("api result:", data);
      });
  }

  private getHttpOptions(){
    const token = this.oidcSecurityService.getToken();

    const httpOptions = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + token,
      }),
    };

    return httpOptions;
  }
}
