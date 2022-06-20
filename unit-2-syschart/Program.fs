namespace syschart

open Akka.FSharp
open Avalonia
open Avalonia.Controls
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI
open Avalonia.FuncUI.Builder
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Hosts
open Avalonia.Themes.Fluent
open ScottPlot.Avalonia
open syschart.Messages

type State =
    | NotInitialized
    | Initiliazed

[<AbstractClass; Sealed>]
type Views =

    static member main actorSystem =
        let chartActor =
            spawn actorSystem "charting" (actorOf Actors.chartingActor)

        let series =
            ChartDataHelper.randomSeries "FakeSeries1" None

        Component (fun ctx ->
            let appState = ctx.useState NotInitialized
            let plotOutlet =
                ctx.useState (null, renderOnChange = false)

            ctx.useEffect (
                handler = (fun () ->
                    match appState.Current with
                    | NotInitialized ->
                        appState.Set Initiliazed
                        chartActor <! InitializeChart (plotOutlet.Current, series)
                    | _ -> ()
                ),
                triggers = [ EffectTrigger.AfterInit ],
                identity = "initialize"
            )

            DockPanel.create [ DockPanel.lastChildFill true
                               DockPanel.children [ ViewBuilder.Create<AvaPlot>([])
                                                    |> View.withOutlet plotOutlet.Set ] ])

type MainWindow() =
    inherit HostWindow()

    do
        base.Title <- "System Metrics"
        base.Width <- 684
        base.Height <- 446

        let chartActors =
            System.create "ChartActors" (Configuration.load ())

        base.Content <- Views.main chartActors

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add(FluentTheme(baseUri = null, Mode = FluentThemeMode.Dark))

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime -> desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

module Program =
    [<EntryPoint>]
    let main args =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)
