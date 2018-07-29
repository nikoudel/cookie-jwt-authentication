This ASP.NET Core MVC application is an example of authentication scheme configuration documented [here](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/limitingidentitybyscheme?view=aspnetcore-2.1&tabs=aspnetcore2x).

The goal is to use cookie-based OpenIdConnect authentication for serving non-API requests (like HTML views) and JWT bearer authentication for state modification requests (POST, PUT etc). One of the benefits of this approach is the API becomes immune to CSRF attacks.

This application is configured to use Auth0 but any other identity provider supporting OIDC can be used as well.

To run the app, create an Auth0 application with the following settings:

```
Allowed Callback URLs: http://localhost:4000/signin-oidc
Allowed Logout URLs:   http://localhost:4000/
```

Start the app by setting environment variables AUTH0_DOMAIN and AUTH0_CLIENTID to corresponding values from the Auth0 app like this:

```
$env:AUTH0_DOMAIN = "<your host>.auth0.com"; $env:AUTH0_CLIENTID = "<your client id>"; dotnet run
```

The Contact page is protected with a cookie and the About page posts to a controller action protected with a bearer token.