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
open OxyPlot
open OxyPlot.Avalonia
open syschart.Messages

[<AbstractClass; Sealed>]
type Views =

    static member main actorSystem =
        let chartActor =
            spawn actorSystem "charting" Actors.chartingActor

        let series =
            ChartDataHelper.randomSeries "FakeSeries 1" None

        Component (fun ctx ->

            let plotModel =
                ctx.useState (PlotModel(), renderOnChange = true)
                
            ctx.useEffect (
                handler =
                    (fun () ->
                        chartActor <! InitializeChart (plotModel.Current, series)
                        plotModel.Set plotModel.Current
                    ),
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
                                Button.verticalAlignment Avalonia.Layout.VerticalAlignment.Bottom
                                Button.margin 5
                                Button.padding (10., 5.)
                                Button.content $"Add Series {plotModel.Current.Series.Count + 1}"
                                Button.onClick (fun _ ->
                                    let newSeriesName = $"FakeSeries {plotModel.Current.Series.Count + 1}"
                                    let newSeries = ChartDataHelper.randomSeries newSeriesName None
                                    chartActor <! AddSeries (plotModel.Current, newSeries)
                                    plotModel.Set plotModel.Current
                                )
                            ]
                        ]
                    ]
                    ViewBuilder.Create<PlotView>([
                        AttrBuilder<PlotView>.CreateProperty<PlotModel>(PlotView.ModelProperty, plotModel.Current, ValueNone)
                    ])
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
        this.Styles.Load("resm:OxyPlot.Avalonia.Themes.Default.xaml?assembly=OxyPlot.Avalonia")

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
