import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { FriendDetailService } from './friend-detail.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';

@Component({
  selector: 'app-friend-detail',
  templateUrl: './friend-detail.component.html',
  styleUrls: ['./friend-detail.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class FriendDetailComponent implements OnInit {

  public friendForm: FormGroup;
  friend: any;
  pageType: string;
  private _unsubscribeAll: Subject<any> = new Subject();

  constructor(
    private _builder: FormBuilder,
    private _router: Router,
    private _matSnackBar: MatSnackBar,
    private _detailService: FriendDetailService
  ) { }

  get f() { return this.friendForm.controls; }

  ngOnInit(): void {

    this._detailService.onFriendChanged
      .pipe(takeUntil(this._unsubscribeAll))
      .subscribe((data) => {
        if (data) {
          this.friend = data;
          this.pageType = 'edit';
        }
        else {
          this.pageType = 'new';
          this.friend = {};
        }

        this.friendForm = this.createForm(this.friend);
      });
  }

  save(): void {
    const request = this.friendForm.getRawValue();
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

        this._matSnackBar.open('Successfully registered friend!', 'OK', {
          verticalPosition: 'bottom',
          duration: 2000
        });

      }).catch((ex) => {

        this._matSnackBar.open('Problems registering friend!', 'OK', {
          verticalPosition: 'bottom',
          duration: 2000
        });
      });
  }

  private update(request) {
    this._detailService.update(request)
      .then(() => {

        this.refresh(request.id);

        this._matSnackBar.open('Friend successfully changed!', 'OK', {
          verticalPosition: 'bottom',
          duration: 2000
        });

      }).catch(() => {
        this._matSnackBar.open('Problems updating friend!', 'OK', {
          verticalPosition: 'bottom',
          duration: 2000
        });
      });
  }
  
  refresh(id: string) {
    this._router.navigate(['friend', id]);
  }

  cancel(): void {
    this._router.navigate(['friend']);
  } 

  private createForm(friend): FormGroup {
    return this._builder.group({
      id: [{ value: friend.id, disabled: this.pageType == 'new' }, Validators.required],
      ver: [{ value: friend.ver, disabled: this.pageType == 'new' }, Validators.required],
      name: [friend.name || '', Validators.required],
      nickname: [friend.nickname || '', Validators.required],
      email: [friend.email || '', Validators.required]
    })
  }
}
