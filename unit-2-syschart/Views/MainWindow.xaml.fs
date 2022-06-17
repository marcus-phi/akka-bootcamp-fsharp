namespace unit_2_syschart.Views

open Avalonia.Controls
open Avalonia.Markup.Xaml
open ScottPlot.Avalonia

type MainWindow() as this =
    inherit Window()
    
    do
        AvaloniaXamlLoader.Load(this)
        let mainPlot = this.Find<AvaPlot>("MainPlot")
        let dataX = [|1.; 2.; 3.; 4.; 5.|]
        let dataY = [|1.; 4.; 9.; 16.; 25.|]
        mainPlot.Plot.AddScatter(dataX, dataY) |> ignore
        mainPlot.Refresh()

