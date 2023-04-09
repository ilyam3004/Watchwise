import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {HomeComponent} from "./modules/home/home.component";
import {AuthGuard} from "./shared/helpers/auth.guard";
import {LoginComponent} from "./modules/auth/login/login.component";

const accountModule = () => import('./modules/auth/auth.module')
  .then(x => x.AuthModule);

const routes: Routes = [
  { path: '', component: HomeComponent},
  { path: '**', redirectTo: ''}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
