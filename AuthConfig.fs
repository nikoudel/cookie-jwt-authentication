module AuthConfig

open System
open Microsoft.AspNetCore.Authentication.OpenIdConnect
open Microsoft.AspNetCore.Authentication.Cookies
open System.Threading.Tasks
open Microsoft.AspNetCore.Authentication.JwtBearer

[<Literal>]
let OidcScheme = "OIDC"
[<Literal>]
let BearerScheme = "Bearer"

let domain = Environment.GetEnvironmentVariable "AUTH0_DOMAIN"
let clientId = Environment.GetEnvironmentVariable "AUTH0_CLIENTID"

if [domain; clientId] |> List.exists String.IsNullOrWhiteSpace then
    failwith "Environment variables must be defined: AUTH0_DOMAIN, AUTH0_CLIENTID"

let setOpenIdConnectOptions (mailbox : Mailbox.Agent) (options: OpenIdConnectOptions) =

    let events = OpenIdConnectEvents()

    events.OnRedirectToIdentityProviderForSignOut <- fun context ->
        let postLogoutUri = Uri.EscapeDataString("http://localhost:4000/")
        let logoutUri = sprintf "https://%s/v2/logout?client_id=%s&returnTo=%s" domain clientId postLogoutUri

        context.Response.Redirect(logoutUri)
        context.HandleResponse()

        Task.CompletedTask

    events.OnTokenValidated <- fun context ->
        mailbox.Value.Post (Mailbox.SetAccessToken context.SecurityToken.RawData)
        Task.CompletedTask

    options.Authority <- "https://" + domain
    options.ClientId <- clientId
    options.ResponseType <- "token id_token"
    options.Events <- events
    options.SignInScheme <- CookieAuthenticationDefaults.AuthenticationScheme

let setJwtBearerOptions (options : JwtBearerOptions) =
    options.Authority <- "https://" + domain
    options.Audience <- clientId