export class BaseComponent {

    ngModelOptions = { standalone: true }
    processing = false
    visible = false

    close() {
        this.visible = false
    }

    error(err) {
        if (err.error && err.error.message) {
            alert(err.error.message)
        } else {
            alert(err.message)
        }
        console.error(err)
        this.setProcessing(false)
    }

    open() {
        this.visible = true
    }

    setProcessing(value: boolean) {
        this.processing = value
    }
}