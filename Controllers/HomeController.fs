namespace NddWeb2.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Authorization
open Microsoft.AspNetCore.Authentication.Cookies

type HomeController (mailbox : Mailbox.Agent) =
    inherit Controller()

    let replyTimeout = 1000

    member this.SetLoggedIn () =
        this.ViewData.["LoggedIn"] <- this.User.Identity.IsAuthenticated

    member this.Index () =
        this.SetLoggedIn()
        this.View()

    member this.About () =
        let comments = mailbox.Value.PostAndReply ((fun ch -> Mailbox.GetComments ch), replyTimeout)
        let accessToken = mailbox.Value.PostAndReply ((fun ch -> Mailbox.GetAccessToken ch), replyTimeout)
        
        if isNull accessToken then
            this.ViewData.["NoToken"] <- "No access token (relogin to get one)"
        else
            this.ViewData.["AccessToken"] <- accessToken

        this.SetLoggedIn()
        this.ViewData.["Message"] <- "Your application description page."
        this.ViewData.["Comments"] <- comments
        this.View()

    [<Authorize(AuthenticationSchemes = AuthConfig.OidcScheme)>]
    member this.Contact () =
        this.SetLoggedIn()
        this.ViewData.["Message"] <- "Your contact page."
        this.View()

    [<HttpPost>]
    [<Authorize(AuthenticationSchemes = AuthConfig.BearerScheme)>]
    member this.AddComment (comment : string) =
        mailbox.Value.Post (Mailbox.AddComment comment)
        printfn "Comment added: %s" comment
        "ok"

    member this.Error () =
        this.SetLoggedIn()
        this.View();

    member this.Logout () =
        this.SignOut [|AuthConfig.OidcScheme; CookieAuthenticationDefaults.AuthenticationScheme|]
