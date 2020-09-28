import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { GameListService } from '../game-list.service';

@Component({
  selector: 'app-loaned',
  templateUrl: './loaned.dialog.html',
  styleUrls: ['./loaned.dialog.scss'],
  encapsulation: ViewEncapsulation.None
})
export class LoanedDialog implements OnInit {

  constructor(
    public dialogRef: MatDialogRef<LoanedDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private _service: GameListService
  ) { }

  ngOnInit(): void {
  }

}
