import { Injectable } from "@angular/core"
import { HttpClient, HttpRequest } from "@angular/common/http"

import { GameInfo } from "./models"

@Injectable()
export class AppService {

    constructor(private http: HttpClient) { }

    postGame(boxArt: File, cia: File, data: GameInfo) {
        let formData = new FormData()
        formData.append("boxArtFile", boxArt)
        formData.append("ciaFile", cia)
        Object.keys(data).forEach(x => {
            formData.append(`data.${x}`, data[x])
        })
        let req = new HttpRequest("POST", "./games", formData, { reportProgress: true })
        return this.http.request(req)
    }
}