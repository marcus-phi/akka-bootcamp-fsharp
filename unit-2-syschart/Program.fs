namespace syschart

open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Hosts
open Avalonia.Themes.Fluent

[<AbstractClass; Sealed>]
type Views =
    
    static member main () =
        Component (fun ctx ->
            DockPanel.create [
            ]
        )
    
type MainWindow() =
    inherit HostWindow()
    do
        base.Title <- "System Metrics"
        base.Width <- 684
        base.Height <- 446
        base.Content <- Views.main ()
        
type App() =
    inherit Application()
    
    override this.Initialize() =
        this.Styles.Add(FluentTheme(baseUri = null, Mode = FluentThemeMode.Dark))
        
    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

module Program =
    [<EntryPoint>]
    let main args =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)
