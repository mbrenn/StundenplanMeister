../deps/datenmeister/assemblies/Release/net7.0/DatenMeister.SourceGeneration.Console.exe "C:\Projekte\StundenplanMeister\src\DatenMeister.StundenPlan\xmi\StundenPlan.Types.xml" C:\Projekte\StundenplanMeister\src\DatenMeister.StundenPlan\Model StundenPlan.Model

# Create .js files
Write-Output ""
Write-Output "Creating Java-Script Files"

Set-Location ..
Set-Location src/DatenMeister.StundenPlan

tsc -p .

Set-Location ../..
Set-Location scripts
