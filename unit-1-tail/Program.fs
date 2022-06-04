open System
open Akka.FSharp
open unit_1_tail

let printInstructions () =
    Console.WriteLine "Write whatever you want into the console!"
    Console.Write "Some lines will appear as"
    Console.ForegroundColor <- ConsoleColor.Red
    Console.Write " red"
    Console.ResetColor()
    Console.Write " and others will appear as"
    Console.ForegroundColor <- ConsoleColor.Green
    Console.Write " green! "
    Console.ResetColor()
    Console.WriteLine()
    Console.WriteLine()
    Console.WriteLine "Type 'exit' to quit this application at any time.\n"

[<EntryPoint>]
let main argv =
    printInstructions ()

    let myActorSystem =
        System.create "MyActorSystem" (Configuration.load ())

    let consoleWriterActor =
        spawn myActorSystem "consoleWriterActor" (actorOf Actors.consoleWriterActor)

    let consoleReaderActor =
        spawn myActorSystem "consoleReaderActor" (actorOf2 (Actors.consoleReadActor consoleWriterActor))

    consoleReaderActor <! Actors.Start

    myActorSystem.WhenTerminated.Wait()
    0
