namespace syschart

open System
open Akka.FSharp.Actors
open ScottPlot
open ScottPlot.Avalonia
open syschart.Types

module Messages =
    type ChartMessage =
        | InitializeChart of plot: AvaPlot * initialSeries: Series
        | AddSeries of plot: AvaPlot * series: Series

open Messages

module Actors =
    let chartingActor (mailbox: Actor<_>) =
        let addScatter (plot: AvaPlot) (series: Series) =
            let newPlottable =
                plot.Plot.AddScatter(series.DataX, series.DataY, lineWidth = 2.f, markerSize = 0.f, label = series.Name)

            plot.Refresh()
            newPlottable

        let rec charting (mapping: Map<string, Plottable.ScatterPlot>) =
            actor {
                let! message = mailbox.Receive()


                let newMapping =
                    match message with
                    | InitializeChart (plot, series) ->
                        plot.Plot.Clear()
                        mapping.Add(series.Name, addScatter plot series)
                    | AddSeries (plot, series) when
                        not <| String.IsNullOrEmpty series.Name
                        && not <| (mapping |> Map.containsKey series.Name)
                        ->
                        mapping.Add(series.Name, addScatter plot series)
                    | _ -> mapping

                return! charting newMapping
            }

        charting Map.empty<string, Plottable.ScatterPlot>
