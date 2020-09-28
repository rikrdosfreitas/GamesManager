import { CollectionViewer, DataSource } from "@angular/cdk/collections";
import { Observable, BehaviorSubject } from 'rxjs';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { MatPaginator } from "@angular/material/paginator";
import { GameListService } from './game-list.service';

export class GameDataSource implements DataSource<any> {

    private gameSubject = new BehaviorSubject<any[]>([]);

    public constructor(private listService: GameListService, private paginator: MatPaginator) { }

    public load(filter: string, sort: string, order: string, page: number, size: number) {
        this.listService.getGames(filter, sort, order, page, size).pipe(
            catchError(_ => {
                return of({ data: [], records: 0 });
            }))
            .subscribe((response: any) => {
                response = response || { data: [], records: 0 };
                
                this.paginator.length = response.records;
                this.gameSubject.next(response.data);
            });
    }

    public connect(collectionViewer: CollectionViewer): Observable<any[]> {
        return this.gameSubject.asObservable();
    }

    public disconnect(collectionViewer: CollectionViewer): void {
        this.gameSubject.complete();
    }
}
