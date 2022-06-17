namespace unit_2_syschart

open System
open Avalonia
open Avalonia.Controls
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Controls.Templates
open Avalonia.Markup.Xaml
open unit_2_syschart.ViewModels
open unit_2_syschart.Views

type ViewLocator() =
    interface IDataTemplate with
        member this.Build(data: obj): IControl =
            let _name = data.GetType().FullName.Replace("ViewModel", "View")
            let _type = Type.GetType(_name)
            if _type <> null then
                Activator.CreateInstance(_type) :?> IControl
            else
                TextBlock( Text = $"Not Found: {_name}") :> IControl
        member this.Match(data: obj) : bool =
            data :? ViewModelBase
            
type App() =
    inherit Application()
    
    override this.Initialize() =
        AvaloniaXamlLoader.Load(this)
        
    override this.OnFrameworkInitializationCompleted() =
        base.OnFrameworkInitializationCompleted()
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()
