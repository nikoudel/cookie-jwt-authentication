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
    member this.AddComment (comment : string) =
        mailbox.Value.Post (Mailbox.AddComment comment)
        printfn "Comment added: %s" comment
        this.Redirect("/Home/About")

    member this.Error () =
        this.SetLoggedIn()
        this.View();

    member this.Logout () =
        this.SignOut [|AuthConfig.OidcScheme; CookieAuthenticationDefaults.AuthenticationScheme|]
