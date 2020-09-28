import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env';

@Injectable({
  providedIn: 'root'
})
export class LentService {
  private _baseUrl = `${environment.apiUrl}/api`;

  constructor(
    private _httpClient: HttpClient
  ) { }

  public get(type: 'games' | 'friends', search = '', sort = '', order = 'asc', page = 0, size = 10): Observable<any> {
    var params = {
      params: new HttpParams()
        .set('search', search)
        .set('sort', sort)
        .set('order', order)
        .set('page', page.toString())
        .set('size', size.toString())
    };

    return this._httpClient.get<any>(`${this._baseUrl}/${type}`, params);
  }

  lent(model): Promise<any> {
    return new Promise((resolve, reject) => {
      this._httpClient.post(`${this._baseUrl}/games/${model.game}/lent`, { id: model.game, friendId: model.friend })
        .subscribe((response: any) => {
          resolve(response);
        }, reject);
    });
  }
}
