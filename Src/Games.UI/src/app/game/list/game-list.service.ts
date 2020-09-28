import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class GameListService {

  private _baseUrl = `${environment.apiUrl}/api/games`;
  public onTenantChanged: BehaviorSubject<any> = new BehaviorSubject({});

  constructor(private _httpClient: HttpClient) { }

  public getGames(search = '', sort = '', order = 'asc', page = 0, size = 10): Observable<any> {
    var params = {
      params: new HttpParams()
        .set('search', search)
        .set('sort', sort)
        .set('order', order)
        .set('page', page.toString())
        .set('size', size.toString())
    };

    return this._httpClient.get<any>(this._baseUrl, params);
  }

  getLoaned(id: any): Promise<any> {
    return new Promise((resolve, reject) => {
      this._httpClient.get(`${this._baseUrl}/${id}/loaned`)
        .subscribe((response: any) => {
          resolve(response);
        }, reject);

    });
  }

  delete(id: any): Promise<any> {
    return new Promise((resolve, reject) => {
      this._httpClient.delete(`${this._baseUrl}/${id}`)
        .subscribe((response: any) => {
          resolve(response);
        }, reject);
    });
  }


  return(id: any): Promise<any> {
    return new Promise((resolve, reject) => {
      this._httpClient.delete(`${this._baseUrl}/${id}/return`)
        .subscribe((response: any) => {
          resolve(response);
        }, reject);
    });
  }
}