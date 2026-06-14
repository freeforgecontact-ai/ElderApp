module Main

open Elmish
open Elmish.React

// Abonnement réseau (online/offline) -> met à jour le bandeau.
let private subscriptionReseau _model =
    let sub dispatch =
        State.ecouterReseau (fun () -> dispatch (State.ReseauChange(State.estEnLigne ())))
        { new System.IDisposable with
            member _.Dispose() = () }
    [ [ "reseau" ], sub ]

// Abonnement minuterie respiration (tick toutes les secondes).
// Actif uniquement lorsqu'un exercice de respiration est en cours.
let private subscriptionResp (model: State.Model) =
    match model.ExoActif, model.Resp with
    | Some Domain.Respiration478, Some _ ->
        let sub dispatch =
            let mutable actif = true
            let rec loop () =
                Fable.Core.JS.setTimeout
                    (fun () ->
                        if actif then
                            dispatch State.TickResp
                            loop ())
                    1000
                |> ignore
            loop ()
            { new System.IDisposable with
                member _.Dispose() = actif <- false }
        [ [ "resp-tick" ], sub ]
    | _ -> []

let private subscriptions model =
    subscriptionReseau model @ subscriptionResp model

Program.mkProgram State.init State.update View.view
|> Program.withSubscription subscriptions
|> Program.withReactSynchronous "app"
|> Program.run
