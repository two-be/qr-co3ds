import { Component, OnInit } from "@angular/core"

import { AppService } from "src/app/app.service"
import { BaseComponent } from "src/app/components"
import { GameInfo } from "src/app/models"

@Component({
    templateUrl: "./admin.page.html"
})
export class AdminPage extends BaseComponent implements OnInit {

    allGames: GameInfo[] = []
    game = new GameInfo()
    games: GameInfo[] = []
    keyword = ""
    visible = false

    constructor(private service: AppService) {
        super()
    }

    add() {
        this.game = new GameInfo()
        this.open()
    }

    close() {
        this.visible = false
    }

    async edit(e: number) {
        try {
            let { ...game } = await this.service.getGame(e).toPromise()
            this.game = game
            this.open()
        } catch (err) {
            this.error(err)
        }
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
        await this.initGames()
    }

    open() {
        this.visible = true
    }

    simpleSearch() {
        let keyword = this.keyword.toUpperCase()
        this.games = this.allGames.filter(x => false
            || x.name.toUpperCase().includes(keyword)
        )
    }
}