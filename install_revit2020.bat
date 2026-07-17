@echo off
setlocal

set "ROOT=%~dp0"
set "DLL=%ROOT%BinhShelfCalculator\bin\x64\Debug\BinhShelfCalculator.dll"
set "ADDIN_DIR=%ProgramData%\Autodesk\Revit\Addins\2020"
set "ADDIN_FILE=%ADDIN_DIR%\BinhShelfCalculator.addin"

if not exist "%DLL%" (
  echo ERROR: Khong tim thay DLL:
  echo %DLL%
  echo.
  echo Hay mo BinhShelfCalculator.sln va Build Debug x64 truoc.
  pause
  exit /b 1
)

if not exist "%ADDIN_DIR%" (
  mkdir "%ADDIN_DIR%"
)

(
  echo ^<?xml version="1.0" encoding="utf-8" standalone="no"?^>
  echo ^<RevitAddIns^>
  echo   ^<AddIn Type="Application"^>
  echo     ^<Name^>Binh Shelf Calculator^</Name^>
  echo     ^<Assembly^>%DLL%^</Assembly^>
  echo     ^<AddInId^>8E6C14B4-47E2-4D20-BBF5-BFA5E5D1A5C1^</AddInId^>
  echo     ^<FullClassName^>BinhShelfCalculator.App^</FullClassName^>
  echo     ^<VendorId^>BINH^</VendorId^>
  echo     ^<VendorDescription^>Binh Shelf Capacity Calculator for Revit 2020^</VendorDescription^>
  echo   ^</AddIn^>
  echo ^</RevitAddIns^>
) > "%ADDIN_FILE%"

echo Da tao file addin:
echo %ADDIN_FILE%
echo.
echo Mo Revit 2020 va tim tab "Binh Shelf".
pause
