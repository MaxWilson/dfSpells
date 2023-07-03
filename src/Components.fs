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

[<AutoOpen>]
module Regex =
    open System.Text.RegularExpressions
    let (|ParseRegex|_|) regex str =
       let m = Regex(regex).Match(str)
       if m.Success
       then Some (List.tail [ for x in m.Groups -> x.Value ])
       else None
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
        Html.div [prop.className "spellHeader"; prop.text $"{spell.name} ({spell.college})"]
        for p, pix in byMagicType |> List.number do
            for chain, cix in p.items |> List.number do
                let prereqs = System.String.Join(" and ", chain)
                let isSelected = choice = (Some(pix, cix))
                let toggle _ =
                    if not isSelected then update (Some(pix, cix))
                    else update None
                class' "prereqs" Html.div [
                    Html.input [prop.type'.checkbox; prop.isChecked isSelected; prop.readOnly true; prop.onClick(toggle)]
                    // don't need to show e.g. "Wizardly" at every single level
                    let descr = if isTopLevel then $"{p.heading}: {prereqs}"
                                else prereqs
                    Html.label [prop.text descr; prop.className "label"; prop.onClick(toggle)]
                    if isSelected then
                        for prereqName in chain do
                            match spellLookup |> Map.tryFind prereqName with
                            | Some spell ->
                                class' "drilldown" Html.div [Spell (spell, Some p.heading)]
                            | None ->
                                match prereqName with
                                | ParseRegex "(.*) including (.*)" [preamble; spellName] ->
                                    class' "drilldown" Html.div [Html.div [prop.className "spellHeader"; prop.text preamble]]
                                    match spellLookup |> Map.tryFind spellName with
                                    | Some spell ->
                                        class' "drilldown" Html.div [Spell (spell, Some p.heading)]
                                    | _ -> failwith $"shouldn't happen {spellName}"
                                | _ ->
                                    class' "drilldown" Html.div [Html.div [prop.className "spellHeader"; prop.text prereqName]]
                    ]
        ]

let colleges = Spells.spells |> List.collect (fun spell -> spell.college.Split("/") |> List.ofArray) |> List.distinct |> List.sort
let debounce ms f =
  let mutable threadId = None
  fun v ->
    if threadId.IsSome then
      Browser.Dom.window.clearTimeout threadId.Value
    threadId <- Browser.Dom.window.setTimeout ((fun _ -> f v; threadId <- None), ms) |> Some

let class' (classname: string) ctor (children: seq<ReactElement>) =
  ctor [prop.className classname; prop.children children]

[<ReactComponent>]
let Spells() =
    let nameFilter, updateNameFilter = React.useState ""
    let allCollege = "All colleges"
    let collegeFilter, updateCollegeFilter = React.useState allCollege
    let nameInput = (debounce 100 updateNameFilter)
    Html.div [
        class' "stickyHeader" Html.div [
          Html.legend "Search:"
          Html.input [
            prop.valueOrDefault nameFilter
            prop.autoFocus true
            prop.onChange nameInput
            ]
          Html.select [
            prop.id "collegeFilter"
            prop.onChange (updateCollegeFilter)
            prop.children [
              Html.option allCollege
              for c in colleges do
                Html.option c
              ]
            prop.value collegeFilter
            ]
          Html.button [
            prop.text "Clear"
            prop.onClick (fun _ ->
              updateCollegeFilter allCollege
              updateNameFilter ""
              nameInput "" // make sure debounce doesn't override the blank filter
              )
            ]
          ]
        class' "body" Html.div [
          Html.h1 "Spells"
          let nameFilter = nameFilter.ToLowerInvariant().Trim()
          let collegeFilter = collegeFilter.ToLowerInvariant().Trim()
          for spell in Spells.spells do
            if (nameFilter = "" || spell.name.ToLowerInvariant().Contains nameFilter)
                && (collegeFilter = "all colleges" || spell.college.ToLowerInvariant().Contains collegeFilter) then
              Spell (spell, None)
          ]
        ]