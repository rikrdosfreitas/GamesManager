import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GameListComponent } from './list/game-list.component';
import { RouterModule } from '@angular/router';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { GameDetailComponent } from './detail/game-detail.component';
import { GameDetailService } from './detail/game-detail.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatButtonModule } from '@angular/material/button';
import { MatRippleModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogModule } from '@angular/material/dialog';
import { ConfirmDialogModule } from '../common/confirm/confirm.module';
import { LentModule } from './list/lent/lent.module';
import { LoanedDialog } from './list/loaned/loaned.dialog';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';

const routes = [
  {
    path: '',
    component: GameListComponent
  },
  {
    path: ':id',
    component: GameDetailComponent,
    resolve: {
      game: GameDetailService
    }
  }
];

@NgModule({
  declarations: [
    GameListComponent,
    GameDetailComponent,
    LoanedDialog
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,

    RouterModule.forChild(routes),
    MatSortModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    FlexLayoutModule,
    MatButtonModule,
    MatRippleModule,
    MatIconModule,
    MatSnackBarModule,
    MatDialogModule,
    MatToolbarModule,
    MatTooltipModule,

    ConfirmDialogModule,
    LentModule

  ],
  entryComponents: [
    LoanedDialog
  ]
})
export class GameModule { }
