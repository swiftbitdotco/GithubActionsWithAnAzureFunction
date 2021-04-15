![netcoreapp3.1](https://github.com/swiftbitdotco/GithubActionsWithAnAzureFunction/actions/workflows/build-and-test.yml/badge.svg?branch=netcoreapp3.1)

# Introduction

An example to build, test and deploy an Azure Function, using Github Actions.

# Prerequisites

You will need to install:

- Dotnet Core 3.1.x SDK (via Visual Studio Installer or by downloading from [here](https://dotnet.microsoft.com/download)
- Azure Functions Core Tools from [here](https://github.com/Azure/azure-functions-core-tools)

Run `dotnet --version` to confirm that Dotnet Core 3.1.x is installed properly.
Run `func --version` to confirm that the Azure Functions Core Tools are installed properly.

# Build & Test

Build with `dotnet build`

Test with `dotnet test`

# Application configuration

## Background

When a dotnet application loads, it loads the following in the following order (the next item will overwrite the previous):

- Host config (Environment variables beginning with `ASPNETCORE_` )
- `appSettings.json`
- `appSettings.Development.json` (or appSettings.{ASPNETCORE_ENVIRONMENT}.json)
- User secrets via `secrets.json` (see below)
- Environment variables
- cmd-line args

All this is true if you are using the `.CreateDefaultBuilder()` method to create your host configuration. If rolling your own — you decide the order.

## Using Environment variables

The `appsettings.json` specifies variables like so:

```json
"TestSettings": {
    "Value1": true
}
```

In C# code, these variables are referenced like this:

```csharp
var value1 = Configuration["TestSettings:Value1"];
```

Environment variables take the same `Key:Value` syntax, but use double underscores (`__`) as a separator, instead of a colon (`:`).

## Windows

Add them to your Environment Variables:

**Note the double underscores**:

```bash
TestSettings__Value1=true
```

### Linux

Add them to your relevant user file (i.e. For Bash users, your file should be `~/.bash_profile`):

**Note the double underscores**:

```bash
export TestSettings__Value1="true"
```

## Using the dotnet Secret Manager tool (safe storage of app secrets in development)

You can use the dotnet Secret Manager tool to store secrets separately from `appsettings.json`.
For more information, you can [read this blog post](https://medium.com/datadigest/user-secrets-in-asp-net-core-with-jetbrains-rider-26c381177391).

Also, [here is a link to the official Microsoft docs](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=linux).

## Running tests locally

The `IntegrationTests` project uses the `secrets.json` pattern.

However, the `IntegrationTests` project has the following specific settings:

```json
"TestSettings": {
    "BaseUrl": "http://localhost:7071/"
},
```

If the `BaseUrl` property has the word `localhost` in it, then the tests will run `func` command (from Azure Functions Core Tools) to spin up the function locally, and fire HTTP calls against that instance.
If the `BaseUrl` property _does not have_ the word `localhost` in it, the tests will _NOT_ run the `func` command (from Azure Functions Core Tools) and instead fire HTTP calls against the given url.

**Note that the `BaseUrl` property should always end with a forward slash (`/`)**.

## Git pre-push script

In the root folder, there is a `pre-push.sh` bash script file. This script runs `dotnet test` before you push to the remote repository, potentially saving yourself a broken build.

To use it, copy the `pre-push.sh` script in the root of the repo to the `.git/hooks` folder (if you can't see a `.git` folder, that's because your OS is hiding hidden files. If on Windows, set Windows explorer to "show hidden folders") and rename the script to `pre-push`.

Or you could run the following command in the root of the repo:

## Windows

Powershell:

```powershell
Copy-Item pre-push.sh -Destination ./.git/hooks/pre-push -Force
```

Bash:

```bash
cp pre-push.sh ./.git/hooks/pre-push
```

## Linux

```bash
chmod a+x pre-push.sh && cp pre-push.sh ./.git/hooks/pre-push
```

To disable the script, simply remove it from the `.git/hooks` folder.

### Notes

- If the script doesn't work, open the file in a text editor like Notepad++ or VS Code and change the line endings to LF/linux (Edit > EOL Conversion > Linux), save the file, and copy it over to the `.git/hooks` folder again.
- If you are using _Fork_ as your git client on a Mac, then you must start it via the command line: `open -a fork` to make it inherit ENV of the parent process. Then both your terminal and Fork will have the same PATH. This enables Fork to use commands like `dotnet`, `npm`, etc.
