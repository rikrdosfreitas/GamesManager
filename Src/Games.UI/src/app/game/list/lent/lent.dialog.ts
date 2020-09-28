import { Component, Inject, Input, OnInit, ViewEncapsulation, OnDestroy, AfterViewInit } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { startWith, map, debounceTime, distinctUntilChanged, filter, takeUntil } from 'rxjs/operators';
import { Observable, Subject } from 'rxjs';
import { LentService } from './lent.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

@Component({
  selector: 'app-lent',
  templateUrl: './lent.dialog.html',
  styleUrls: ['./lent.dialog.scss'],
  encapsulation: ViewEncapsulation.None
})
export class LentDialog implements OnInit, OnDestroy, AfterViewInit {

  private _onDestroy$: Subject<void> = new Subject();

  lentForm: FormGroup;
  gameCtrl = new FormControl("", Validators.required);
  friendCtrl = new FormControl("", Validators.required);

  gameOptions: Observable<ISearch[]>;
  friendOptions: Observable<ISearch[]>;

  constructor(
    private _builder: FormBuilder,
    public dialogRef: MatDialogRef<LentDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private _service: LentService
  ) {

  }

  get f() { return this.lentForm.controls; }

  ngOnInit() {

    this.lentForm = this._builder.group({
      "game": ['', Validators.required],
      "friend": ['', Validators.required]
    });

    this.gameCtrl.valueChanges
      .pipe(
        debounceTime(500),
        distinctUntilChanged(),
        takeUntil(this._onDestroy$),
        filter((x) => x != undefined && x.length >= 3))
      .subscribe(() => this.onGameSearch());

    this.friendCtrl.valueChanges
      .pipe(
        debounceTime(500),
        distinctUntilChanged(),
        takeUntil(this._onDestroy$),
        filter((x) => x != undefined && x.length >= 3))
      .subscribe(() => this.onFriendSearch());

  }

  ngOnDestroy() {
    this._onDestroy$.next();
    this._onDestroy$.complete();
  }

  ngAfterViewInit(): void {

    if (!this.data.game) return;

    this.gameCtrl.patchValue(this.data.game);
    let ctrl = this.lentForm.get('game');
    ctrl.patchValue(this.data.game.id);
    ctrl.markAsDirty();

  }

  save(): void {
    const request = this.lentForm.getRawValue();
    this._service.lent(request)
      .then((response: string) => {
        this.dialogRef.close(true);
      }).catch((ex) => {
      });
  }

  /* Game Search */
  onGameSelected(event: MatAutocompleteSelectedEvent): void {
    const value = event.option.value as ISearch;

    if ((value.id || '').trim()) {
      let ctrl = this.lentForm.get('game');
      ctrl.patchValue(value.id);
      ctrl.markAsDirty();
    }
  }

  displayGame(game: ISearch): string {
    return game && game.name ? game.name : '';
  }

  onGameSearch() {
    if (this.gameCtrl && this.gameCtrl.dirty) {
      const filter: string = this.gameCtrl.value;
      this.getGames(filter);
    }
  }

  getGames(filter): void {
    this.gameOptions = this._service.get('games', filter).pipe(map(x => x.data as ISearch[]));
  }

  /* Friend Search */
  onFriendSelected(event: MatAutocompleteSelectedEvent): void {
    const value = event.option.value as ISearch;

    if ((value.id || '').trim()) {
      let ctrl = this.lentForm.get('friend');
      ctrl.patchValue(value.id);
      ctrl.markAsDirty();
    }
  }

  displayFriend(friend: ISearch): string {
    return friend && friend.name ? friend.name : '';
  }

  onFriendSearch() {
    if (this.friendCtrl && this.friendCtrl.dirty) {
      const filter: string = this.friendCtrl.value;
      this.getFriends(filter);
    }
  }

  getFriends(filter): void {
    this.friendOptions = this._service.get('friends', filter).pipe(map(x => x.data as ISearch[]));
  }


}

export interface ISearch {
  id: string;
  name: string;
}
