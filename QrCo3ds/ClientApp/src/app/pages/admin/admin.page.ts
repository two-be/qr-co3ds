import { Component, OnInit } from "@angular/core"
import { HttpEventType, HttpResponse } from "@angular/common/http"
import * as moment from "moment"
import * as numeral from "numeral"
import { Observable } from "rxjs"

import { AppService } from "src/app/app.service"
import { BaseComponent } from "src/app/components"
import { CategoryInfo, DlcInfo, GameInfo, ScreenshotInfo } from "src/app/models"

@Component({
    templateUrl: "./admin.page.html"
})
export class AdminPage extends BaseComponent implements OnInit {

    allGames: GameInfo[] = []
    boxArtFile: File
    categories: string[] = [
        "Action",
        "Adventure",
        "Application",
        "Education",
        "Fitness",
        "Indie",
        "Music",
        "Party",
        "Puzzle",
        "Racing",
        "Role-Playing",
        "Side-Scrolling",
        "Simulation",
        "Sports",
        "Strategy",
    ]
    ciaFile: File
    dlc = new DlcInfo()
    dlcIndex = 0
    dlcs: DlcInfo[] = []
    game = new GameInfo()
    games: GameInfo[] = []
    keyword = ""
    isVisible = false
    percent = 0
    progressMessage = ""
    regions: string[] = [
        "EUR",
        "Free",
        "JPN",
        "USA",
    ]
    removedCategories: CategoryInfo[] = []
    screenshotFiles: File[] = []
    selectedCategories: string[] = []

    constructor(private service: AppService) {
        super()
    }

    add() {
        this.dlc = new DlcInfo()
        this.dlcs = []
        this.game = new GameInfo()
        this.selectedCategories = []
        this.open()
    }

    boxArtChange(e) {
        this.boxArtFile = e.target.files[0]
    }

    categoryChange(e) {
        this.removedCategories = this.game.categories.filter(x => !e.includes(x.name))
    }

    ciaChange(e) {
        this.ciaFile = e.target.files[0]
    }

    async delete(e: GameInfo) {
        try {
            if (!confirm(`Delete ${e.name}?`)) {
                return
            }
            this.setProcessing(true)
            await this.service.deleteGame(e.id).toPromise()
            this.allGames = this.allGames.filter(x => x.id != e.id)
            this.simpleSearch()
            this.setProcessing(false)
        } catch (err) {
            this.error(err)
        }
    }

    async deleteDlc(e: DlcInfo) {
        try {
            if (!confirm(`Delete ${e.name}?`)) {
                return
            }
            this.setProcessing(true)
            await this.service.deleteDlc(e.id).toPromise()
            this.dlcs = this.dlcs.filter(x => x.id != e.id)
            this.setProcessing(false)
        } catch (err) {
            this.error(err)
        }
    }

    async deleteScreenshot(e: ScreenshotInfo) {
        try {
            if (!confirm(`Delete ${e.fileName}?`)) {
                return
            }
            this.setProcessing(true)
            await this.service.deleteScreenshot(e.id).toPromise()
            this.game.screenshots = this.game.screenshots.filter(x => x.id != e.id)
            this.setProcessing(false)
        } catch (err) {
            this.error(err)
        }
    }

    dlcFileChange(e) {
        this.dlc.$file = e.target.files[0]
    }

    async edit(e: number) {
        try {
            this.setProcessing(true)
            let { ...game } = await this.service.getGame(e).toPromise()
            this.selectedCategories = game.categories.map(x => x.name)
            this.dlcs = game.dlcs
            this.game = game
            this.open()
            this.setProcessing(false)
        } catch (err) {
            this.error(err)
        }
    }

    editDlc(e: DlcInfo) {
        if (e.id) {
            e.$id = e.id
        }
        let { ...dlc } = e
        this.dlc = dlc
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
        await this.initGames()
    }

    async save() {
        if (!this.game.name) {
            alert("Please enter a valid name.")
            return
        }
        if (!this.game.releaseDate) {
            alert("Please enter a valid release date.")
            return
        }
        this.showModal()
        this.game.categories = this.selectedCategories.map(x => {
            let category = this.game.categories.find(y => y.name == x) || new CategoryInfo()
            category.gameId = this.game.id
            category.name = x
            return category
        })
        this.game.releaseDate = moment(this.game.releaseDate).format("YYYY-MM-DD")
        let observable: Observable<any>
        if (this.game.id) {
            observable = this.service.putGame(this.boxArtFile, this.ciaFile, this.game)
            await this.service.deleteCategories(this.removedCategories.map(x => x.id)).toPromise()
            await this.service.postCategories(this.game.categories.filter(x => x.id == 0)).toPromise()
        } else {
            observable = this.service.postGame(this.boxArtFile, this.ciaFile, this.game)
        }
        observable.subscribe(e => {
            if (e.type == HttpEventType.UploadProgress) {
                this.progressMessage = `${this.game.name} - ${numeral(e.loaded).format("0,0")} bytes of ${numeral(e.total).format("0,0")} bytes`
                this.percent = Math.round(100 * e.loaded / e.total)
            } else if (e instanceof HttpResponse) {
                let game: any = e.body
                if (this.dlcs.length) {
                    this.saveDlcs(game)
                } else if (this.screenshotFiles.length) {
                    this.saveScreenshots(game)
                } else {
                    this.saveComplete(game)
                }
            }
        }, err => {
            this.error(err)
            this.handleCancel()
        })
    }

    saveComplete(e: GameInfo) {
        let game = this.allGames.find(x => x.id == e.id)
        if (game) {
            game.name = e.name
        } else {
            this.allGames = [e, ...this.allGames]
        }
        this.simpleSearch()
        this.handleCancel()
        this.close()
    }

    saveDlc() {
        let dlc = this.dlcs.find(x => x.$id == this.dlc.$id)
        if (!dlc) {
            this.dlc.fileName = this.dlc.$file.name
            this.dlcs = [...this.dlcs, this.dlc]
        } else {
            dlc.$file = this.dlc.$file
            dlc.fileName = this.dlc.$file.name
            dlc.name = this.dlc.name
        }
        this.dlc = new DlcInfo()
    }

    saveDlcs(game: GameInfo) {
        let x = this.dlcs[this.dlcIndex]
        if (x) {
            x.gameId = game.id
            let observable: Observable<any>
            if (x.id) {
                observable = this.service.putDlc(x.$file, x)
            } else {
                observable = this.service.postDlc(x.$file, x)
            }
            observable.subscribe(e => {
                if (e.type == HttpEventType.UploadProgress) {
                    this.progressMessage = `${x.name} - ${numeral(e.loaded).format("0,0")} bytes of ${numeral(e.total).format("0,0")} bytes`
                    this.percent = Math.round(100 * e.loaded / e.total)
                } else if (e instanceof HttpResponse) {
                    this.dlcIndex++
                    this.saveDlcs(game)
                }
            }, err => {
                this.error(err)
            })
        } else if (this.screenshotFiles.length) {
            this.saveScreenshots(game)
            this.dlcIndex = 0
        } else {
            this.saveComplete(game)
            this.dlcIndex = 0
        }
    }

    saveScreenshots(game: GameInfo) {
        this.service.postScreenshots(this.screenshotFiles, game.id).subscribe(e => {
            if (e.type == HttpEventType.UploadProgress) {
                this.progressMessage = `Screenshots - ${numeral(e.loaded).format("0,0")} bytes of ${numeral(e.total).format("0,0")} bytes`
                this.percent = Math.round(100 * e.loaded / e.total)
            } else if (e instanceof HttpResponse) {
                this.saveComplete(game)
            }
        }, err => {
            this.error(err)
        })
    }

    screenshotsChange(e) {
        this.screenshotFiles = e.target.files
    }

    showModal() {
        this.isVisible = true
    }

    simpleSearch() {
        let keyword = this.keyword.toUpperCase()
        this.games = this.allGames.filter(x => false
            || x.name.toUpperCase().includes(keyword)
            || x.tagName.toUpperCase().includes(keyword)
        )
    }
}