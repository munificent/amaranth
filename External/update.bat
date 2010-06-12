rem Copies the external dependency .dlls into this directory.
rem Assumes that the .dlls are already built and are in the same root directory
rem as the Amaranth project.

copy "..\..\bramble\bin\Release\Bramble.Core.dll"
copy "..\..\malison\bin\Release\Malison.Core.dll"
copy "..\..\malison\bin\Release\Malison.WinForms.dll"
