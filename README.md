[![license](https://img.shields.io/github/license/secana/penet.svg)](https://raw.githubusercontent.com/secana/PeNet/master/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/PeNet.svg)](https://www.nuget.org/packages/PeNet/)
[![NuGet](https://img.shields.io/nuget/dt/PeNet.svg)](https://www.nuget.org/packages/PeNet/)
[![Build](https://img.shields.io/azure-devops/build/secana/PeNet/2.svg)](https://dev.azure.com/secana/PeNet/_build?definitionId=2)
[![Test](https://img.shields.io/azure-devops/tests/secana/PeNet/2.svg)](https://dev.azure.com/secana/PeNet/_build?definitionId=2)
[![BuiltWithDot.Net shield](https://builtwithdot.net/project/492/penet-csharp-net-standard-file-parser/badge)](https://builtwithdot.net/project/492/penet-csharp-net-standard-file-parser)

![PeNet Logo](https://raw.githubusercontent.com/secana/PeNet/master/resource/linkedin_banner_image_2.png "PeNet - PE analysis made easy")
PeNet is a parser for Windows Portable Executable headers. It completely written in C# and does not rely on any native Windows APIs.
Furthermore it supports the creation of Import Hashes (ImpHash), which is a feature often used in malware analysis. You can extract Certificate Revocation List, compute different hash sums and other useful stuff for working with PE files.

## Getting Started & API Reference

The API reference can be found here: [PeNet Documentation & API Reference](http://secana.github.io/PeNet).

For an overview of *PeNet* or to analyze PE files go to: [penet.io](http://penet.io)

## Continuous Integration

The project is automatically build, tested and released with an [Azure Pipeline](https://dev.azure.com/secana/PeNet).

To release a new version, push a tagged commit. For example:

 ```powershell
 git tag -a v2.0.0 -m 'Release version 2.0.0'
 git push origin v2.0.0
 ```
