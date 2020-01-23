import { HttpEventType, HttpResponse } from "@angular/common/http"
import { Component, OnInit } from "@angular/core"

import { AppService } from "../app.service"
import { BaseComponent } from "../components/base.component"
import { GameInfo } from "../models"

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
})
export class HomeComponent extends BaseComponent implements OnInit {

  allGames: GameInfo[] = []
  boxArt: File
  cia: File
  isVisible = false
  game = new GameInfo()
  games: GameInfo[] = []
  keyword = ""
  qrData = ""

  constructor(private service: AppService) {
    super()
  }

  boxArtChange(e) {
    this.boxArt = e.target.files[0]
  }

  ciaChange(e) {
    this.cia = e.target.files[0]
  }

  handleCancel() {
    this.isVisible = false
  }

  async initGames() {
    try {
      this.games = await this.service.getGames().toPromise()
      this.allGames = this.games
    } catch (err) {
      this.error(err)
    }
  }

  async ngOnInit() {
    this.setProcessing(true)
    await this.initGames()
    this.setProcessing(false)
  }

  showModal(e: GameInfo) {
    this.game = e
    this.qrData = `${window.location.origin}${e.ciaUrl.substring(1)}`
    this.isVisible = true
  }

  simpleSearch() {
    let keyword = this.keyword.toUpperCase()
    this.games = this.allGames.filter(x => false
      || x.name.toUpperCase().includes(keyword)
    )
  }

  upload() {
    let game = new GameInfo()
    game.developer = "Beenox"
    game.gameplayUrl = "https://www.youtube.com/watch?v=slwCk2-vVxY"
    game.name = "The Amazing Spider-Man"
    game.numberOfPlayer = 1
    game.publisher = "Activision"
    game.releaseDate = "2012-06-26"
    this.service.postGame(this.boxArt, this.cia, game).subscribe(e => {
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
