export class BaseComponent {

    processing = false

    error(err) {
        if (err.error && err.error.message) {
            alert(err.error.message)
        } else {
            alert(err.message)
        }
        console.error(err)
        this.setProcessing(false)
    }

    setProcessing(value: boolean) {
        this.processing = value
    }
}