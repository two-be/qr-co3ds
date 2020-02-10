import { Injectable } from "@angular/core"
import { HttpClient, HttpRequest } from "@angular/common/http"

import { CategoryInfo, DlcInfo, GameInfo, ScreenshotInfo } from "./models"
import { route } from "./utilities"

@Injectable()
export class AppService {

    constructor(private http: HttpClient) { }

    private getApi(route: string) {
        return `./${route}`
    }

    deleteCategories(ids: number[]) {
        let url = this.getApi(`${route.category}?`)
        ids.forEach(x => {
            url += `ids=${x}&`
        })
        return this.http.delete(url)
    }

    deleteDlc(id: number) {
        return this.http.delete(this.getApi(`${route.dlc}/${id}`))
    }

    deleteGame(id: number) {
        return this.http.delete(this.getApi(`${route.game}/${id}`))
    }

    deleteScreenshot(id: number) {
        return this.http.delete(this.getApi(`${route.screenshot}/${id}`))
    }

    postCategories(body: CategoryInfo[]) {
        return this.http.post<CategoryInfo[]>(this.getApi(`${route.category}`), body)
    }

    postDlc(cia: File, data: DlcInfo) {
        let formData = new FormData()
        formData.append("ciaFile", cia)
        formData.append("json", JSON.stringify(data))
        let req = new HttpRequest("POST", this.getApi(route.dlc), formData, { reportProgress: true })
        return this.http.request(req)
    }

    getGame(id: number) {
        return this.http.get<GameInfo>(this.getApi(`${route.game}/${id}`))
    }

    getGames() {
        return this.http.get<GameInfo[]>(this.getApi(route.game))
    }

    postGame(boxArt: File, cia: File, data: GameInfo) {
        let formData = new FormData()
        formData.append("boxArtFile", boxArt)
        formData.append("ciaFile", cia)
        formData.append("json", JSON.stringify(data))
        let req = new HttpRequest("POST", this.getApi(route.game), formData, { reportProgress: true })
        return this.http.request(req)
    }

    postScreenshots(files: File[], gameId: number) {
        let formData = new FormData()
        Array.from(files).forEach((x, i) => {
            formData.append(`file${i}`, x)
        })
        let req = new HttpRequest("POST", this.getApi(`${route.screenshot}/game/${gameId}`), formData, { reportProgress: true })
        return this.http.request(req)
    }

    putDlc(cia: File, data: DlcInfo) {
        let formData = new FormData()
        formData.append("ciaFile", cia)
        formData.append("json", JSON.stringify(data))
        let req = new HttpRequest("PUT", this.getApi(`${route.dlc}/${data.id}`), formData, { reportProgress: true })
        return this.http.request(req)
    }

    putGame(boxArt: File, cia: File, data: GameInfo) {
        let formData = new FormData()
        formData.append("boxArtFile", boxArt)
        formData.append("ciaFile", cia)
        formData.append("json", JSON.stringify(data))
        let req = new HttpRequest("PUT", this.getApi(`${route.game}/${data.id}`), formData, { reportProgress: true })
        return this.http.request(req)
    }
}