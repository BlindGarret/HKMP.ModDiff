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

#### Client Installation

Get the latest HKMP.ModDiff.zip from the current release and add sll of its contents to a mod folder in your mods directory like
``` sh
<Path to Hollow Knight>/hollow_knight_Data/Managed/Mods/HKMP.ModdDiff
```

#### Server Installation

If you are planning to run your server through the Hollow Knight game the client installation is sufficient. If however you want to run it in standalone EXE mode, drop the contents of HKMP.ModDiff.zip next to your server exe. Then boot up Hollow Knight and copy over the modlist.json it generates in your mod directory into your server directory as well. This json acts as the standard all people attempting to connect to the server must abide by.


## Configuration
All configurations are hot-reload ready. If you make a modification to the config file the server will attempt to hot reload the changes in real time.

### moddiff_config.json

Upon first starting up a file "moddiff_config.json" will be generated inside the server/mod directory. This is a simple JSON file which can be modified in real time to update server settings.

| Configuration Name  | Description                                                                                                                                                                                                                                           | Data type | Default |
|---------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------|---------|
| MismatchOnExtraMods | Flag for whether the addon considers extra mods on a client to be a mismatch. If turned on it will go into "strict" mode about mod matching.                                                                                                          | Boolean   | False   |
| KickOnMistmatch     | Flag for whether the server will auto-kick client who connect with a mismatched mod list. Currently the kick will happen about 500ms after the connection in order to not trap the connection in a weird state and leave behind ghosts of the player. | Boolean   | False   |

<!-- GETTING STARTED -->
## Development

### Project Setup

For simplicity this project uses the LocalBuildProperties.props file in the Hkmp.ModDiff directory to find it's references to the HK-Modding API and HKMP. There is an example file (LocalBuildProperties_example.props) to be used to setup the props file. Once setup you are ready to use your IDE of choice and build.

### Contributing

This is a completely open source project. Feel free to make Pull Requests with any changes you wish to make. You may also open issues on the repo for others to attempt to fix.

### Style Guide

This project uses the standard [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions) with the addendum that you should use type inference (var) by default unless there is an explicit reason not to.  
