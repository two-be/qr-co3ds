import { Injectable } from "@angular/core"
import { HttpClient } from "@angular/common/http"

import { GameInfo } from "./models"

@Injectable()
export class AppService {

    constructor(private http: HttpClient) { }

    postGame(boxArt: File, cia: File, data: GameInfo) {
        let formData = new FormData()
        formData.append("boxArt", boxArt)
        formData.append("cia", cia)
        Object.keys(data).forEach(x => {
            formData.append(`data.${x}`, data[x])
        })
        return this.http.post("./game", formData, { reportProgress: true })
    }
}