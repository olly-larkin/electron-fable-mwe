(*
    JSHelpers.fs

    Some JS related utility functions.
*)

module JSHelpers

open Electron

let mutable debugLevel = 0

/// Hack to provide a constant global variable
/// set from command line arguments of main process.
/// 0 => production. 1 => dev. 2 => debug.
let setDebugLevel() =
    let argV =
        electron.remote.``process``.argv
        |> Seq.toList
        |> (function | [] -> [] | _ :: args' -> args')
        |> List.map (fun s -> s.ToLower())
    let isArg s = List.contains s argV

    if isArg "--debug" || isArg "-d" then
        debugLevel <- 2
    elif isArg "-w" then
        debugLevel <- 1