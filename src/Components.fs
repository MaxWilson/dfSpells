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

open Spells
[<ReactComponent>]
let Spell (spell:Spell) =
    Html.div [
        Html.h3 spell.name
        for p in spell.prereqs do
            let class' (className: string) ctor (children: _ list) = ctor [prop.children children; prop.className className]
            let classTxt (className: string) ctor (txt: string) = ctor [prop.text txt; prop.className className]
            for chain in p.items do
                let prereqs = System.String.Join(" and ", chain)
                classTxt "prereq" Html.div $"{p.heading}: {prereqs}"
        ]

[<ReactComponent>]
let Spells() =
    Html.div [
        Html.h1 "Spells"
        for spell in Spells.spells do
            Spell spell
        ]