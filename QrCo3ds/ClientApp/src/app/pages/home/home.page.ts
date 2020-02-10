import { Component, OnInit } from "@angular/core"

import { AppService } from "src/app/app.service"
import { BaseComponent } from "src/app/components"
import { DlcInfo, GameInfo } from "src/app/models"

@Component({
    selector: "app-home",
    templateUrl: "./home.page.html",
})
export class HomePage extends BaseComponent implements OnInit {

    allGames: GameInfo[] = []
    isVisible = false
    game = new GameInfo()
    games: GameInfo[] = []
    keyword = ""
    qrData = ""

    constructor(private service: AppService) {
        super()
    }

    getQrData(e: DlcInfo): string {
        return `${window.location.origin}${e.fileUrl.substring(1)}`
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

    async showInfo(e: GameInfo) {
        try {
            this.setProcessing(true)
            this.game = await this.service.getGame(e.id).toPromise()
            this.visible = true
            this.setProcessing(false)
        } catch (err) {
            this.error(err)
        }
    }

    async showModal(e: GameInfo) {
        try {
            this.setProcessing(true)
            this.game = await this.service.getGame(e.id).toPromise()
            this.qrData = `${window.location.origin}${e.ciaUrl.substring(1)}`
            this.isVisible = true
            this.setProcessing(false)
        } catch (err) {
            this.error(err)
        }
    }

    simpleSearch() {
        let keyword = this.keyword.toUpperCase()
        this.games = this.allGames.filter(x => false
            || x.name.toUpperCase().includes(keyword)
            || x.tagName.toUpperCase().includes(keyword)
        )
    }
}
