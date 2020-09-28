import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-confirm',
  templateUrl: './confirm.dialog.html',
  styleUrls: ['./confirm.dialog.scss']
})
export class ConfirmDialog  {

  public confirmMessage: string;

  constructor(
    public dialogRef: MatDialogRef<ConfirmDialog>
  ) { }
}