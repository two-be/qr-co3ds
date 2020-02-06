import { Injectable } from "@angular/core"
import { HttpClient, HttpRequest } from "@angular/common/http"

import { GameInfo } from "./models"
import { route } from "./utilities"

@Injectable()
export class AppService {

    constructor(private http: HttpClient) { }

    private getApi(route: string) {
        return `./${route}`
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
}