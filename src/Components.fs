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

let spellLookup = spells |> List.map (fun s -> s.name, s) |> Map.ofList

[<ReactComponent>]
let rec Spell (spell:Spell, magicType: MagicType option) =
    let byMagicType = spell.prereqs |> List.filter (fun p -> magicType = None || magicType = Some p.heading)
    let isTopLevel = magicType.IsNone
    let tryOpen() =
        match byMagicType with
        | [{ items = [_onlyOneChain] }] -> Some(0,0) // if there's only one possible option, choose it
        | _ -> None
    let choice, update = React.useState(if isTopLevel then None else tryOpen())
    Html.div [
        Html.div [prop.className "spellHeader"; prop.text spell.name]
        for p, pix in byMagicType |> List.number do
            for chain, cix in p.items |> List.number do
                let prereqs = System.String.Join(" and ", chain)
                let isSelected = choice = (Some(pix, cix))
                let toggle _ =
                    if not isSelected then update (Some(pix, cix))
                    else update None
                class' "prereqs" Html.div [
                    Html.input [prop.type'.checkbox; prop.isChecked isSelected; prop.onClick(toggle)]
                    // don't need to show e.g. "Wizardly" at every single level
                    let descr = if isTopLevel then $"{p.heading}: {prereqs}"
                                else prereqs
                    Html.label [prop.text descr; prop.className "label"; prop.onClick(toggle)]
                    if isSelected then
                        for prereqName in chain do
                            match spellLookup |> Map.tryFind prereqName with
                            | Some spell ->
                                class' "drilldown" Html.div [Spell (spell, Some p.heading)]
                            | None -> ()
                    ]
        ]

[<ReactComponent>]
let Spells() =
    Html.div [
        Html.h1 "Spells"
        for spell in Spells.spells do
            Spell (spell, None)
        ]