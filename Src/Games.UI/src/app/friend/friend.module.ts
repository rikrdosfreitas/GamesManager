import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FriendListComponent } from './list/friend-list.component';
import { RouterModule } from '@angular/router';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { FriendDetailComponent } from './detail/friend-detail.component';
import { FriendDetailService } from './detail/friend-detail.service';
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

const routes = [
  {
    path: '',
    component: FriendListComponent
  },
  {
    path: ':id',
    component: FriendDetailComponent,
    resolve: {
      friend: FriendDetailService
    }
  }
];

@NgModule({
  declarations: [FriendListComponent, FriendDetailComponent],
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
    
    ConfirmDialogModule
  ]
})
export class FriendModule { }
