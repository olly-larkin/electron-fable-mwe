(*
    ModelType.fs

    This module provides the type for the FRP UI.
    It is not possible to put this type among the CommonTypes as it has to
    depend on Draw2dWrapper. Furthermore, non-UI modules should be agnostic of
    the FRP model.
*)

module rec ModelType

type Msg =
    | Msg of string

type Model = { DisplayString: string } with
    static member init = { DisplayString = "Hello, world!" }