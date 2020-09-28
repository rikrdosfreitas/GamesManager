import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AutoLoginComponent } from './auto-login/auto-login.component';
const routes = [
  { path: 'autologin', component: AutoLoginComponent },  
  {
    path: 'friend',
    loadChildren: () => import('./friend/friend.module').then(m => m.FriendModule)
  },
  {
    path: 'game',
    loadChildren: () => import('./game/game.module').then(m => m.GameModule)
  }
];


@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: true })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
