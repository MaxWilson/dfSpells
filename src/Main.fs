module Main

open Feliz
open App
open Browser.Dom
open Fable.Core.JsInterop

importSideEffects "./styles.sass"

ReactDOM.render(
    Components.Spells(),
    document.getElementById "feliz-app"
)