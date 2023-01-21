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


type SpellData = {
    data: Spell
    chains: PotentialChain list
    }
and PotentialChain = {
    heading: MagicType
    items: PrereqData list list
}
and PrereqData = Expanded of Spell | Unexpanded of string

[<ReactComponent>]
let Spell (spell:SpellData) =
    Html.div [
        Html.h3 spell.data.name
        for p in spell.chains do
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