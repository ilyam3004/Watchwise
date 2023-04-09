import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthRoutingModule } from './auth-routing.module';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { LayoutComponent } from './layout/layout.component';
import {ReactiveFormsModule} from "@angular/forms";


@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent,
    LayoutComponent
  ],
  imports: [
    CommonModule,
    AuthRoutingModule,
    ReactiveFormsModule,
  ]
})
export class AuthModule { }
