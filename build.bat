SETLOCAL ENABLEDELAYEDEXPANSION

rmdir /s /q releases 
md releases

rmdir /s /q Pickaxe.Console\bin\release 
dotnet build
dotnet test

set winOS=win-x64
set unixOS=osx-x64 linux-x64
for %%N in (%winOS%) do (
	set rid=%%N
	dotnet publish Pickaxe.Console\Pickaxe.Console.csproj -f netcoreapp3.1 -c Release -r !rid! -o releases\frameworkdependent\!rid! --self-contained false
	dotnet publish Pickaxe.Console\Pickaxe.Console.csproj -f netcoreapp3.1 -c Release -r !rid! -o releases\selfcontained\!rid! --self-contained true
	"C:\Program Files\7-Zip\7z.exe" a -tzip .\releases\pickaxe-!rid!.zip .\releases\frameworkdependent\!rid!\* -r
	"C:\Program Files\7-Zip\7z.exe" a -tzip .\releases\self-contained-pickaxe-!rid!.zip .\releases\selfcontained\!rid!\* -r
)

for %%N in (%unixOS%) do (
	set rid=%%N
	dotnet publish Pickaxe.Console\Pickaxe.Console.csproj -f netcoreapp3.1 -c Release -r !rid! -o releases\frameworkdependent\!rid! --self-contained false
	dotnet publish Pickaxe.Console\Pickaxe.Console.csproj -f netcoreapp3.1 -c Release -r !rid! -o releases\selfcontained\!rid! --self-contained true
	"C:\Program Files\7-Zip\7z.exe" a -ttar -so .\releases\pickaxe-!rid!.tar .\releases\frameworkdependent\!rid!\* -r | "C:\Program Files\7-Zip\7z.exe" a -si -tgzip .\releases\pickaxe-!rid!.tar.gz
	"C:\Program Files\7-Zip\7z.exe" a -ttar -so .\releases\self-contained-pickaxe-!rid!.tar .\releases\selfcontained\!rid!\* -r | "C:\Program Files\7-Zip\7z.exe" a -si -tgzip .\releases\self-contained-pickaxe-!rid!.tar.gz
)

