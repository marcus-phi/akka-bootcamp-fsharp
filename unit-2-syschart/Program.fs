namespace syschart

open Akka.FSharp
open Avalonia
open Avalonia.Controls
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI
open Avalonia.FuncUI.Builder
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Hosts
open Avalonia.Layout
open Avalonia.Themes.Fluent
open ScottPlot.Avalonia
open syschart.Messages

[<AbstractClass; Sealed>]
type Views =

    static member main actorSystem =
        let chartActor =
            spawn actorSystem "charting" Actors.chartingActor

        let series =
            ChartDataHelper.randomSeries "FakeSeries1" None

        Component (fun ctx ->

            let plotOutlet =
                ctx.useState (null, renderOnChange = false)

            ctx.useEffect (
                handler =
                    (fun () ->
                        chartActor
                        <! InitializeChart (plotOutlet.Current, series)),
                triggers = [ EffectTrigger.AfterInit ],
                identity = "initialize"
            )

            DockPanel.create [
                DockPanel.lastChildFill true
                DockPanel.children [
                    DockPanel.create [
                        DockPanel.dock Dock.Right
                        DockPanel.children [
                            Button.create [
                                Button.dock Dock.Bottom
                                Button.verticalAlignment VerticalAlignment.Bottom
                                Button.margin 5
                                Button.padding (10., 5.)
                                Button.content "Add Series"
                                Button.onClick (fun _ ->
                                    let newSeriesName = $"FakeSeries {plotOutlet.Current.Plot.GetPlottables().Length + 1}"
                                    let newSeries = ChartDataHelper.randomSeries newSeriesName None
                                    chartActor <! AddSeries (plotOutlet.Current, newSeries)
                                )
                            ]
                        ]
                    ]
                    ViewBuilder.Create<AvaPlot>([])
                    |> View.withOutlet plotOutlet.Set
                ]
            ])

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
