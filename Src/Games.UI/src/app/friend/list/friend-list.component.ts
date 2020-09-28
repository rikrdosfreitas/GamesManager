import { Component, OnInit, ViewChild, AfterViewInit, OnDestroy, ViewEncapsulation, ElementRef } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FriendDataSource } from './friend.datasource';
import { FriendListService } from './friend-list.service';
import { Router } from '@angular/router';
import { Subject, Subscription, fromEvent, merge } from 'rxjs';
import { takeUntil, map, debounceTime, distinctUntilChanged, filter } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { ConfirmDialog } from '../../common/confirm/confirm.dialog';

@Component({
  selector: 'app-friend-list',
  templateUrl: './friend-list.component.html',
  styleUrls: ['./friend-list.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class FriendListComponent implements OnInit {

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild('search', { static: true }) search: ElementRef;

  private _onDestroy$: Subject<any> = new Subject();

  displayedColumns = ['name', 'nickname', 'email', 'actions'];
  datasource: FriendDataSource;
  onChange: Subscription;
  confirmDialogRef: MatDialogRef<ConfirmDialog>;
  constructor(
    private _listService: FriendListService,
    private _router: Router,
    private _matDialog: MatDialog,
    private _matSnackBar: MatSnackBar
  ) { }

  ngOnInit(): void {

    this.datasource = new FriendDataSource(this._listService, this.paginator);
    this.sort.sortChange
      .pipe(takeUntil(this._onDestroy$))
      .subscribe(() => this.paginator.pageIndex = 0);

    fromEvent(this.search.nativeElement, 'keyup')
      .pipe(
        map((e: any) => e.target.value),
        debounceTime(500),
        distinctUntilChanged(),
        takeUntil(this._onDestroy$),
        filter((x) => x.length >= 3 || x == '')
      ).subscribe(() => {
        this.paginator.pageIndex = 0;
        this.loadPages();
      });

    const displayDataChanges = [
      this.paginator.page,
      this.sort.sortChange
    ];

    this.onChange = merge(...displayDataChanges)
      .pipe(takeUntil(this._onDestroy$))
      .subscribe(() => {
        this.loadPages()
      });
  }

  ngAfterViewInit(): void {
    this.loadPages();
  }

  ngOnDestroy(): void {
    this._onDestroy$.next();
    this._onDestroy$.complete();
  }

  public new(): void {
    this._router.navigate(['friend', 'new']);
  }

  public edit(row): void {
    this._router.navigate(['friend', row.id]);
  }

  public remove(row): void {
    this.confirmDialogRef = this._matDialog.open(ConfirmDialog, {
      disableClose: true
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Do you really want to delete?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (!result) { return }

      this._listService.delete(row.id)
        .then(() => {
          this._matSnackBar.open('Friend successfully deleted!', 'OK', {
            verticalPosition: 'bottom',
            duration: 2000
          });
          this.loadPages();
        })
    });
  }

  public loadPages() {
    var filter = this.search.nativeElement.value;
    var sort = this.sort.active;
    var order = this.sort.direction;
    var page = this.paginator.pageIndex;
    var size = this.paginator.pageSize;
    this.datasource.load(filter, sort, order, page, size);
  }

}
