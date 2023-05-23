import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";

import { AppComponent } from "./app.component";
import { FlightModule } from './flight/flight.module';
import { HttpClientModule } from "@angular/common/http";
import { RouterModule } from "@angular/router";
import { FLIGHT_ROUTES } from "./flight/flight.routes";

@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule,
    FlightModule,
    HttpClientModule,
    RouterModule.forRoot([...FLIGHT_ROUTES])],
  bootstrap: [AppComponent]
})
export class AppModule { }
