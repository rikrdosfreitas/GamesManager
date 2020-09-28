import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { GameDetailService } from './game-detail.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';

@Component({
  selector: 'app-game-detail',
  templateUrl: './game-detail.component.html',
  styleUrls: ['./game-detail.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class GameDetailComponent implements OnInit {

  public gameForm: FormGroup;
  game: any;
  pageType: string;
  private _unsubscribeAll: Subject<any> = new Subject();

  constructor(
    private _builder: FormBuilder,
    private _router: Router,
    private _matSnackBar: MatSnackBar,
    private _detailService: GameDetailService
  ) { }

  get f() { return this.gameForm.controls; }

  ngOnInit(): void {

    this._detailService.onGameChanged
      .pipe(takeUntil(this._unsubscribeAll))
      .subscribe((data) => {
        if (data) {
          this.game = data;
          this.pageType = 'edit';
        }
        else {
          this.pageType = 'new';
          this.game = {};
        }

        this.gameForm = this.createForm(this.game);
      });
  }

  save(): void {
    const request = this.gameForm.getRawValue();
    switch (this.pageType) {
      case 'edit':
        this.update(request);
        break;
      case 'new':
        this.create(request);
        break
      default:
        break;
    }
  }

  private create(request: any) {
    this._detailService.save(request)
      .then((response: string) => {

        this.refresh(response);

        this._matSnackBar.open('Successfully registered game!', 'OK', {
          verticalPosition: 'bottom',
          duration: 2000
        });

      }).catch((ex) => {

        this._matSnackBar.open('Problems registering game!', 'OK', {
          verticalPosition: 'bottom',
          duration: 2000
        });
      });
  }

  private update(request) {
    this._detailService.update(request)
      .then(() => {

        this.refresh(request.id);

        this._matSnackBar.open('Game successfully changed!', 'OK', {
          verticalPosition: 'bottom',
          duration: 2000
        });

      }).catch(() => {
        this._matSnackBar.open('Problems updating game!', 'OK', {
          verticalPosition: 'bottom',
          duration: 2000
        });
      });
  }

  refresh(id: string) {
    this._router.navigate(['game', id]);
  }

  cancel(): void {
    this._router.navigate(['game']);
  }

  private createForm(game): FormGroup {
    return this._builder.group({
      id: [{ value: game.id, disabled: this.pageType == 'new' }, Validators.required],
      ver: [{ value: game.ver, disabled: this.pageType == 'new' }, Validators.required],
      name: [game.name || '', Validators.required],
      launchYear: [game.launchYear || '', Validators.required],
      platform: [game.platform || '', Validators.required]
    })
  }
}
