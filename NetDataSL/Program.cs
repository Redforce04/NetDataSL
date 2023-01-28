// See https://aka.ms/new-console-template for more information
//dotnet publish -r win-x64 -c Release
//dotnet publish -r linux-arm64 -c Release
using NetDataSL;

float refreshTime = 5f;
if (args.Length > 0)
    float.TryParse(args[0], out refreshTime);

var unused = new Plugin(refreshTime);