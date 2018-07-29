module Mailbox

open Microsoft.Extensions.Logging

type Message =
    | AddComment of string
    | GetComments of AsyncReplyChannel<string seq>

let messageLoop (inbox: MailboxProcessor<Message>) =

    let rec loop comments = async {

        let! message = inbox.Receive()

        match message with
        | AddComment comment ->
            return! loop (comment :: comments)
        | GetComments ch ->
            ch.Reply comments
            return! loop comments
    }

    loop List.empty

type Agent (log: ILogger<Agent>) =
    let store = MailboxProcessor.Start messageLoop
    do store.Error.Add(fun e -> log.LogError(e, "Mailbox agent failed"))
    member this.Value with get () = store
