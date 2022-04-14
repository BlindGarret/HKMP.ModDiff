<!-- PROJECT LOGO -->
<br />
<p align="center">
  <a href="https://github.com/BlindGarret/HKMP.ModDiff">
    <img src="images/logo.png" alt="Logo" width="300">
  </a>

  <p align="center">
    HKMP.ModDiff
    <br />
    <a href="https://github.com/BlindGarret/HKMP.ModDiff/issues">Report Bug</a> |
    <a href="https://github.com/BlindGarret/HKMP.ModDiff/issues">Request Feature</a>
  </p>
</p>

### Built With

* [DotNet Framwork 4.7.2](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472)
* [HKMP](https://github.com/Extremelyd1/HKMP)
* [HK-Modding](https://hk-modding.github.io/api/api/index.html)

## Description

This is an HKMP Addon which gives you controls to check for modlist mismatches between the connecting clients and the server. It allows you to configure how close a client must match the server, and also whether mismatching clients should be auto-kicked from the server. This is designed to be a helpful tool to ensure that clients and servers have the same mods installed to make setting up multiplayer sessions reliant on those mods easier.


## Installation

### Prerequisites

This addon requires the HK-Modding API, and HKMP to be installed. The referenced version of HKMP will be included with each release notes, but is generally the latest version.

### Manual installation

Get the latest HKMP.ModDiff.dll from the current release and add it to your HKMP mod folder in 
``` sh
<Path to Hollow Knight>/hollow_knight_Data/Managed/Mods/HKMP
```

<!-- GETTING STARTED -->
## Development

### Project Setup

For simplicity this project uses the LocalBuildProperties.props file in the Hkmp.ModDiff directory to find it's references to the HK-Modding API and HKMP. There is an example file (LocalBuildProperties_example.props) to be used to setup the props file. Once setup you are ready to use your IDE of choice and build.

### Contributing

This is a completely open source project. Feel free to make Pull Requests with any changes you wish to make. You may also open issues on the repo for others to attempt to fix.

### Style Guide

This project uses the standard [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions) with the addendum that you should use type inference (var) by default unless there is an explicit reason not to.  
