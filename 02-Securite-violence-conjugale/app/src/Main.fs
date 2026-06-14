module Main

open Elmish
open Elmish.React

// Abonnement à l'état réseau (online/offline) -> met à jour le bandeau.
let private subscription _model =
    let sub dispatch =
        State.ecouterReseau (fun () -> dispatch (State.ReseauChange(State.estEnLigne ())))
        { new System.IDisposable with
            member _.Dispose() = () }
    [ [ "reseau" ], sub ]

Program.mkProgram State.init State.update View.view
|> Program.withSubscription subscription
|> Program.withReactSynchronous "app"
|> Program.run
