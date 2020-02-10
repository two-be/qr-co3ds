class AbstractBase {
    id = 0
}

export class CategoryInfo extends AbstractBase {
    gameId = 0
    name = ""
}

export class DlcInfo extends AbstractBase {
    $file: File
    fileName = ""
    fileUrl = ""
    gameId = 0
    $id = new Date().getTime()
    name = ""
}

export class GameInfo extends AbstractBase {
    boxArtFile = ""
    boxArtUrl = ""
    categories: CategoryInfo[] = []
    ciaFile = ""
    ciaUrl = ""
    dlcs: DlcInfo[] = []
    developer = ""
    gameplayUrl = ""
    isLegit = false
    name = ""
    numberOfPlayers = 0
    publisher = ""
    region = ""
    releaseDate = ""
    screenshots: ScreenshotInfo[] = []
    tagName = ""
}

export class ScreenshotInfo extends AbstractBase {
    fileName = ""
    fileUrl = ""
    gameId = 0
}