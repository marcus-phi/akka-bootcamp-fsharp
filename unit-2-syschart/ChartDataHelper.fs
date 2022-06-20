module syschart.ChartDataHelper

open Akka.Util
open Types

let randomSeries (seriesName: string) (points: int option) =
    let seriesPoints = defaultArg points 100

    let dataX =
        [ 0..seriesPoints ]
        |> List.toArray
        |> Array.map float

    let dataY =
        [ 0..seriesPoints ]
        |> List.toArray
        |> Array.map (fun i ->
            let rng = ThreadLocalRandom.Current.NextDouble()
            float (2.0 * sin (rng) + sin (rng / 4.5)))

    { Name = seriesName
      DataX = dataX
      DataY = dataY }
