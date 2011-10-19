mkdir ReleaseX86
copy /Y .\README /A .\ReleaseX86 /A
copy /Y .\Documentation\README.pdf /B .\ReleaseX86 /B
copy /Y .\KAET\Resources\Licenses\gpl-3.0.txt .\ReleaseX86
copy /Y .\SetupX86\Release\KAETSetupX86.msi .\ReleaseX86
copy /Y .\SetupX86\Release\setup.exe .\ReleaseX86

mkdir ReleaseX64
copy /Y .\README /A .\ReleaseX64 /A
copy /Y .\Documentation\README.pdf /B .\ReleaseX64 /B
copy /Y .\KAET\Resources\Licenses\gpl-3.0.txt .\ReleaseX64
copy /Y .\SetupX64\Release\KAETSetupX64.msi .\ReleaseX64
copy /Y .\SetupX64\Release\setup.exe .\ReleaseX64
PAUSE