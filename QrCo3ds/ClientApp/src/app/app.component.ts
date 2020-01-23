import { Component } from "@angular/core"

import { AppService } from "./app.service"
import { GameInfo } from "./models"
import { HttpEventType, HttpResponse } from "@angular/common/http"

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html"
})
export class AppComponent {

  boxArt: File
  cia: File
  keyword = ""

  constructor(private service: AppService) { }

  boxArtChange(e) {
    this.boxArt = e.target.files[0]
  }

  ciaChange(e) {
    this.cia = e.target.files[0]
  }

  upload() {
    let game = new GameInfo()
    game.developer = "Beenox"
    game.gameplayUrl = "https://www.youtube.com/watch?v=slwCk2-vVxY"
    game.name = "The Amazing Spider-Man"
    game.numberOfPlayer = 1
    game.publisher = "Activision"
    game.releaseDate = "2012-06-26"
    this.service.postGame(this.boxArt, this.cia, game).subscribe((e: any) => {
      if (e.type == HttpEventType.UploadProgress) {
        const percentDone = Math.round(100 * e.loaded / e.total)
        console.log(`File is ${percentDone}% uploaded.`)
      } else if (e instanceof HttpResponse) {
        console.log("File is completely uploaded!")
      }
    }, err => {
      console.log(err)
    })
  }
}
