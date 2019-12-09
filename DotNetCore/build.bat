dotnet build
dotnet test
dotnet publish -f netcoreapp2.2 -r win-x64 --self-contained false
dotnet publish -f netcoreapp2.2 -r linux-x64 --self-contained false
dotnet publish -f netcoreapp2.2 -r osx-x64 --self-contained false