module App.Components
open Feliz
open Feliz.Router

[<ReactComponent>]
let Counter() =
    let (count, setCount) = React.useState(0)
    Html.div [
        Html.h1 count
        Html.button [
            prop.onClick (fun _ -> setCount(count + 1))
            prop.text "Increment"
        ]
    ]

[<ReactComponent>]
let Spells() =
    Html.div [
        Html.h1 "Spells"
        for spell in Spells.spells do
            Html.h3 spell.name
        ]