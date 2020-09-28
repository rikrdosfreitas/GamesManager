import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { ConfirmDialog } from './confirm.dialog';

@NgModule({
    declarations: [
        ConfirmDialog
    ],
    imports: [
        MatDialogModule,
        MatButtonModule
    ],
    entryComponents: [
        ConfirmDialog

    ],
})
export class ConfirmDialogModule
{
}