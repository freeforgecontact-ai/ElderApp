module Main

open Fable.Core
open Elmish
open Elmish.React

// Touche Échap = sortie rapide (geste de sécurité, fonctionne partout).
[<Emit("document.addEventListener('keydown', function(e){ if (e.key === 'Escape') { $0(); } })")>]
let private ecouterEchap (cb: unit -> unit) : unit = jsNative

let private subscription _model =
    let sub dispatch =
        ecouterEchap (fun () -> dispatch State.SortieRapide)
        { new System.IDisposable with
            member _.Dispose() = () }
    [ [ "echap" ], sub ]

Program.mkProgram State.init State.update View.view
|> Program.withSubscription subscription
|> Program.withReactSynchronous "app"
|> Program.run
