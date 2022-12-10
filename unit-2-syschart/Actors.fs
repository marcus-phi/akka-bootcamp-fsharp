namespace syschart

open System
open Akka.FSharp.Actors
open OxyPlot
open OxyPlot.Series
open syschart.Types

module Messages =
    type ChartMessage =
        | InitializeChart of plot: PlotModel * initialSeries: Series
        | AddSeries of plot: PlotModel * series: Series

open Messages

module Actors =
    let chartingActor (mailbox: Actor<_>) =
        let addScatter (plot: PlotModel) (series: Series) =
            let lineSeries = LineSeries(Title = series.Name)
            Array.zip series.DataX series.DataY
            |> Array.iter (fun (x, y) -> lineSeries.Points.Add(DataPoint(x, y)))
            plot.Series.Add(lineSeries)
            plot.InvalidatePlot(true)
            lineSeries

        let rec charting (mapping: Map<string, Series.Series>) =
            actor {
                let! message = mailbox.Receive()

                let newMapping =
                    match message with
                    | InitializeChart (plotModel, series) ->
                        mapping.Add(series.Name, addScatter plotModel series)
                    | AddSeries (plotModel, series) when
                        not <| String.IsNullOrEmpty series.Name
                        && not <| (mapping |> Map.containsKey series.Name)
                        ->
                        mapping.Add(series.Name, addScatter plotModel series)
                    | _ -> mapping

                return! charting newMapping
            }

        charting Map.empty<string, Series.Series>
