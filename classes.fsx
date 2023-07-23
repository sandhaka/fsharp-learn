// Classes usage (ConsolePrompt and Person)

open System

type Person(name : string, color : string) =
    let mutable favoriteColor = "(None)"
    
    do
        if String.IsNullOrWhiteSpace(name) then
            raise <| ArgumentException("Null or empty", "name")
        if not <| String.IsNullOrWhiteSpace(color) then
            favoriteColor <- color.Trim()
    
    member this.Description() =
        $"Name: {name}, favorite color: {favoriteColor}"
        

type ConsolePrompt(message : string, maxTries : int) =
    do
        if String.IsNullOrWhiteSpace(message) then
            raise <| ArgumentException("Null or empty", "message")
    let trimmedMessage = message.Trim()
    let mutable tryCount = 0
 
    let mutable foreground = ConsoleColor.White
    let mutable background = ConsoleColor.Black
 
    member this.ColorScheme
        with get() =
            foreground, background
        and set(fg, bg) =
            if (fg = bg) then
                raise <| InvalidOperationException("Foreground and background colors must be different")
            foreground <- fg
            background <- bg
 
    member this.GetValue() =
        tryCount <- tryCount + 1
        Console.ForegroundColor <- foreground
        Console.BackgroundColor <- background
        printf "%s: " trimmedMessage 
        Console.ResetColor()
        let input = Console.ReadLine()
        if String.IsNullOrWhiteSpace(input) && tryCount < maxTries then
            if this.BeepOnError then
                Console.Beep()
            this.GetValue()
        else
            input
 
    member val BeepOnError = true
        with get, set
        

let namePrompt = ConsolePrompt("Please enter your name", 3)
let color = ConsolePrompt("Please enter your favorite color", 3)
let person = Person(namePrompt.GetValue(), color.GetValue())
namePrompt.BeepOnError <- false
namePrompt.ColorScheme <- ConsoleColor.White, ConsoleColor.Green
printfn "%s" (person.Description())
0