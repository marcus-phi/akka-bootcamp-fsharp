open System
open Akka.FSharp
open unit_1_tail

[<EntryPoint>]
let main argv =
    let myActorSystem =
        System.create "MyActorSystem" (Configuration.load ())

    let consoleWriterActor =
        spawn myActorSystem "consoleWriterActor" (actorOf Actors.consoleWriterActor)

    let consoleReaderActor =
        spawn myActorSystem "consoleReaderActor" (actorOf2 (Actors.consoleReadActor consoleWriterActor))

    consoleReaderActor <! Messages.Start

    myActorSystem.WhenTerminated.Wait()
    0
