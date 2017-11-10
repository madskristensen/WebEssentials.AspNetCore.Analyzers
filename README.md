# Roslyn Analyzer for ASP.NET Core

[![Build status](https://ci.appveyor.com/api/projects/status/mq3u27vpjc22f9se?svg=true)](https://ci.appveyor.com/project/madskristensen/webessentials-aspnetcore-analyzers)
[![NuGet](https://img.shields.io/nuget/v/WebEssentials.AspNetCore.Analyzers.svg)](https://nuget.org/packages/WebEssentials.AspNetCore.Analyzers/)

## Analyzers in this package

### Razor Page handler async void
Page handlers in Razor Pages model classes must not be async void.

### Middleware ordering
Ensures that known middleware that is sensitive to ordering is correctly ordered.