namespace NddWeb2

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Authentication.OpenIdConnect
open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.Extensions.Logging


type Startup private () =
    new (configuration: IConfiguration, mailboxLogger : ILogger<Mailbox.Agent>) as this =
        Startup() then
        this.Configuration <- configuration
        this.MailboxLogger <- mailboxLogger

    member val Configuration : IConfiguration = null with get, set
    member val MailboxLogger : ILogger<Mailbox.Agent> = null with get, set

    member this.ConfigureServices(services: IServiceCollection) =
        let agent = Mailbox.Agent(this.MailboxLogger)

        services
            .AddSingleton(agent)
            .AddAuthentication()
            .AddCookie()
            .AddOpenIdConnect(AuthConfig.OidcScheme, Action<OpenIdConnectOptions> (AuthConfig.setOpenIdConnectOptions agent))
            .AddJwtBearer(AuthConfig.BearerScheme, Action<JwtBearerOptions> AuthConfig.setJwtBearerOptions) |> ignore
            |> ignore

        services.AddMvc() |> ignore

    member this.Configure(app: IApplicationBuilder, env: IHostingEnvironment) =

        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore
        else
            app.UseExceptionHandler("/Home/Error") |> ignore

        app
            .UseStaticFiles()
            .UseAuthentication()
            .UseMvc(fun routes ->
                routes.MapRoute(
                    name = "default",
                    template = "{controller=Home}/{action=Index}/{id?}") |> ignore
                )
            |> ignore
