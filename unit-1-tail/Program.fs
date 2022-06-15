open System
open Akka.Actor
open Akka.FSharp
open unit_1_tail

[<EntryPoint>]
let main argv =
    let myActorSystem =
        System.create "MyActorSystem" (Configuration.load ())

    let consoleWriterActor =
        spawn myActorSystem "consoleWriterActor" (actorOf Actors.consoleWriterActor)

    let strategy () =
        Strategy.OneForOne(
            (fun ex ->
                match ex with
                | :? ArithmeticException -> Directive.Resume
                | :? NotSupportedException -> Directive.Stop
                | _ -> Directive.Restart),
            10,
            TimeSpan.FromSeconds(30.)
        )

    let tailCoordinatorActor =
        spawnOpt
            myActorSystem
            "tailCoordinatorActor"
            (actorOf2 Actors.tailCoordinatorActor)
            [ SpawnOption.SupervisorStrategy(strategy ()) ]

    let fileValidationActor =
        spawn
            myActorSystem
            "validationActor"
            (actorOf2 (Actors.fileValidationActor consoleWriterActor tailCoordinatorActor))

    let consoleReaderActor =
        spawn myActorSystem "consoleReaderActor" (actorOf2 (Actors.consoleReadActor fileValidationActor))

    consoleReaderActor <! Messages.Start

    myActorSystem.WhenTerminated.Wait()
    0
