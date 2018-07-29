namespace NddWeb2.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Authorization
open Microsoft.AspNetCore.Authentication.Cookies

type HomeController () =
    inherit Controller()

    member this.SetLoggedIn () =
        this.ViewData.["LoggedIn"] <- this.User.Identity.IsAuthenticated

    member this.Index () =
        this.SetLoggedIn()
        this.View()

    member this.About () =
        this.SetLoggedIn()
        this.ViewData.["Message"] <- "Your application description page."
        this.View()

    [<Authorize(AuthenticationSchemes = AuthConfig.OidcScheme)>]
    member this.Contact () =
        this.SetLoggedIn()
        this.ViewData.["Message"] <- "Your contact page."
        this.View()

    member this.Error () =
        this.SetLoggedIn()
        this.View();

    member this.Logout () =
        this.SignOut [|AuthConfig.OidcScheme; CookieAuthenticationDefaults.AuthenticationScheme|]
