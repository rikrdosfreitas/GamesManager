import { Injectable } from '@angular/core';
import { environment } from '@env';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class GameDetailService implements Resolve<any>  {


  private _baseUrl = `${environment.apiUrl}/api/games`;
  routeParams: any;
  model: any;
  onGameChanged: BehaviorSubject<any>;

  constructor(
    private _httpClient: HttpClient
  ) {
    this.onGameChanged = new BehaviorSubject({});
  }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {
    this.routeParams = route.params;
    return new Promise((resolve, reject) => {
      Promise.all([
        this.get()
      ]).then(
        () => {
          resolve();
        },
        reject
      );
    });
  }

  get(): Promise<any> {
    return new Promise((resolve, reject) => {
      if (this.routeParams.id === 'new') {
        this.onGameChanged.next(false);
        resolve(false);
      }
      else {
        this._httpClient.get(`${this._baseUrl}/${this.routeParams.id}`)
          .subscribe((response: any) => {
            this.model = response;
            this.onGameChanged.next(this.model);
            resolve(response);
          }, reject);
      }
    });
  }

  save(model): Promise<any> {
    return new Promise((resolve, reject) => {
      this._httpClient.post(`${this._baseUrl}`, model)
        .subscribe((response: any) => {
          resolve(response);
        }, reject);
    });
  }

  update(model): Promise<any> {
    return new Promise((resolve, reject) => {
      this._httpClient.put(`${this._baseUrl}/${model.id}`, model)
        .subscribe((response: any) => {
          resolve(response);
        }, reject);
    });
  }
}