import { CollectionViewer, DataSource } from "@angular/cdk/collections";
import { Observable, BehaviorSubject } from 'rxjs';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { MatPaginator } from "@angular/material/paginator";
import { FriendListService } from './friend-list.service';

export class FriendDataSource implements DataSource<any> {

    private friendSubject = new BehaviorSubject<any[]>([]);

    public constructor(private listService: FriendListService, private paginator: MatPaginator) { }

    public load(filter: string, sort: string, order: string, page: number, size: number) {
        this.listService.getFriends(filter, sort, order, page, size).pipe(
            catchError(_ => {
                return of({ data: [], records: 0 });
            }))
            .subscribe((response: any) => {
                response = response || { data: [], records: 0 };
                
                this.paginator.length = response.records;
                this.friendSubject.next(response.data);
            });
    }

    public connect(collectionViewer: CollectionViewer): Observable<any[]> {
        return this.friendSubject.asObservable();
    }

    public disconnect(collectionViewer: CollectionViewer): void {
        this.friendSubject.complete();
    }
}
