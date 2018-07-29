namespace NddWeb2

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Authentication.OpenIdConnect


type Startup private () =
    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration

    member this.ConfigureServices(services: IServiceCollection) =
        services
            .AddAuthentication()
            .AddCookie()
            .AddOpenIdConnect(AuthConfig.OidcScheme, Action<OpenIdConnectOptions> AuthConfig.setOpenIdConnectOptions)
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

    member val Configuration : IConfiguration = null with get, set
