module Mailbox

open Microsoft.Extensions.Logging

type Message =
    | AddComment of string
    | GetComments of AsyncReplyChannel<string seq>
    | SetAccessToken of string
    | GetAccessToken of AsyncReplyChannel<string>

let messageLoop (inbox: MailboxProcessor<Message>) =

    let rec loop comments token = async {

        let! message = inbox.Receive()

        match message with
        | AddComment comment ->
            return! loop (comment :: comments) token
        | GetComments ch ->
            ch.Reply comments
            return! loop comments token
        | SetAccessToken newToken ->
            return! loop comments newToken
        | GetAccessToken ch ->
            ch.Reply token
            return! loop comments token
    }

    loop List.empty null

type Agent (log: ILogger<Agent>) =
    let store = MailboxProcessor.Start messageLoop
    do printfn "Agent started..."
    do store.Error.Add(fun e -> log.LogError(e, "Mailbox agent failed"))
    member this.Value with get () = store
