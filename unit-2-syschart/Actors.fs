namespace syschart

open ScottPlot.Avalonia
open syschart.Types

module Messages =
    type InitializeChart = InitializeChart of plot: AvaPlot * initialSeries: Series

open Messages

module Actors =
    let chartingActor message =
        match message with
        | InitializeChart (plot, series) ->
            plot.Plot.Clear()

            plot.Plot.AddScatter(series.DataX, series.DataY, lineWidth = 2.f, markerSize = 0.f, label = series.Name)
            |> ignore

            plot.Plot.Legend()
            |> ignore
