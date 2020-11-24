module Renderer

open Elmish
open Elmish.React
open Elmish.Debug
open Elmish.HMR

open Fable.React
open Electron
open ModelType

open Fable.SimpleJson

let exitApp() =
    electron.ipcRenderer.send("exit-the-app",[||])  

// -- Init Model

let init() = 
    JSHelpers.setDebugLevel()
    Model.init, Cmd.none

// -- Create View

let view model dispatch = div [] [ str model.DisplayString ]

// -- Update Model

let update msg model = model, Cmd.none

printfn "Starting renderer..."

Program.mkProgram init update view
|> Program.withReactBatched "app"
|> Program.run