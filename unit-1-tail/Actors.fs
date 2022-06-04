namespace unit_1_tail

open Akka.Actor
open Akka.FSharp
open System

module Actors =
    type Command =
        | Start
        | Continue
        | Message of string
        | Exit
        
    let (|Message|Exit|) (str: string) =
        match str.ToLower() with
        | "exit" -> Exit
        | _ -> Message(str)
        
    let consoleReadActor (consoleWriter: IActorRef) (mailbox: Actor<_>) message =
        let line = Console.ReadLine ()
        match line with
        | Exit -> mailbox.Context.System.Terminate() |> ignore
        | Message(input) -> ()
        
    let consoleWriterActor message =
        let (|Even|Odd|) n = if n % 2 = 0 then Even else Odd
        
        let printInColor color message =
            Console.ForegroundColor <- color
            Console.WriteLine(message.ToString())
            Console.ResetColor()
        
        match message.ToString().Length with
        | 0 -> printInColor ConsoleColor.DarkYellow "Please provide an input.\n"
        | Even -> printInColor ConsoleColor.Red "Your string had an even # of characters.\n"
        | Odd -> printInColor ConsoleColor.Green "Your string had an odd # of characters.\n"