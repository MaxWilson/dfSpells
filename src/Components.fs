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

module Tup2 =
    let create x y = x,y
module List =
    let number lst = lst |> List.mapi (fun ix v -> v, ix)

[<AutoOpen>]
module Html =
    let class' (className: string) ctor (children: _ list) = ctor [prop.children children; prop.className className]
    let classTxt (className: string) ctor (txt: string) = ctor [prop.text txt; prop.className className]

[<ReactComponent>]
let Spell (spell:Spell) =
    let choice, update = React.useState(None)
    Html.div [
        Html.h3 spell.name
        for p, pix in spell.prereqs |> List.number do
            for chain, cix in p.items |> List.number do
                let prereqs = System.String.Join(" and ", chain)
                match choice with
                | Some choice when choice <> (pix, cix) -> ()
                | _ ->
                    let toggle _ =
                        if choice.IsNone then update (Some(pix, cix))
                        else update None
                    class' "prereqs" Html.div [
                        Html.input [prop.type'.checkbox; prop.isChecked choice.IsSome; prop.onClick(toggle)]
                        Html.label [prop.text $"{p.heading}: {prereqs} {(pix, cix)}"; prop.className "label"; prop.onClick(toggle)]
                        ]
        ]

[<ReactComponent>]
let Spells() =
    Html.div [
        Html.h1 "Spells"
        for spell in Spells.spells do
            Spell spell
        ]