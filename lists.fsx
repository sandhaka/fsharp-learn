open System.Drawing
 
type ColorHistory(initialColors : seq<Color>, maxLength : int) = 
    let mutable colors = 
        initialColors
        |> Seq.truncate maxLength
        |> List.ofSeq
    member this.Colors() =
        colors
        |> Seq.ofList
    member this.Add(color : Color) =
        let colors' =
            color :: colors
            |> List.distinct
            |> List.truncate maxLength
        colors <- colors'
    member this.TryLatest() =
        match colors with
        | head::_ ->
            head
            |> Some
        | [] ->
            None
    member this.RemoveLatest() =
        match colors with
        | _::tail ->
            colors <- tail
        | [] ->
            ()
            
 
let listColors (history : ColorHistory) =
    history.Colors()
    |> Seq.iter (printf "%A ")
    printfn ""
 
    
printf "I can create a color history with some colors :"
let history = ColorHistory([Color.Indigo; Color.Violet], 7)
history
|> listColors

printfn "I can add a color:"
history.Add(Color.Blue)
history |> listColors
printfn "The new color is the latest: "
printfn "%O" (history.TryLatest())

printfn "I can re add an existing color - it is 'moved' to latest: "
history.Add(Color.Indigo)
history
|> listColors

printfn "When I add colors beyond the maximum length the oldest is/are pushed out: "
[Color.Green; Color.Yellow; Color.Orange; Color.Red; Color.PeachPuff]
|> List.iter (history.Add)
history
|> listColors

printfn "I can remove the latest color: "
history.RemoveLatest()
history
|> listColors

0    