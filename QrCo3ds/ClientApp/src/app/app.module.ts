import { registerLocaleData } from "@angular/common"
import { HttpClientModule } from "@angular/common/http"
import en from "@angular/common/locales/en"
import { NgModule } from "@angular/core"
import { FormsModule } from "@angular/forms"
import { BrowserModule } from "@angular/platform-browser"
import { BrowserAnimationsModule } from "@angular/platform-browser/animations"
import { RouterModule } from "@angular/router"
import { QRCodeModule } from "angularx-qrcode"
import { NgZorroAntdModule, NZ_I18N, en_US } from "ng-zorro-antd"

import { AppComponent } from "./app.component"
import { AppService } from "./app.service"
import {
  AdminPage,
  HomePage,
} from "./pages"

registerLocaleData(en)

@NgModule({
  declarations: [
    AdminPage,
    AppComponent,
    HomePage,
  ],
  imports: [
    BrowserAnimationsModule,
    BrowserModule.withServerTransition({ appId: "ng-cli-universal" }),
    FormsModule,
    HttpClientModule,
    NgZorroAntdModule,
    QRCodeModule,
    RouterModule.forRoot([
      { path: "", component: HomePage, pathMatch: "full" },
      { path: "admin", component: AdminPage },
    ], { useHash: true }),
  ],
  providers: [
    { provide: NZ_I18N, useValue: en_US },
    AppService,
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
