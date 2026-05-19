@echo off
setlocal

echo ==========================================
echo Building AdaptivePoints Dynamo Package
echo ==========================================

REM Root directory
set ROOT_DIR=%~dp0

REM Project path
set PROJECT=%ROOT_DIR%src\AdaptivePoints\AdaptivePoints\AdaptivePoints.csproj

REM Clean old build
echo.
echo Cleaning build folder...
rmdir /s /q "%ROOT_DIR%build" 2>nul

REM Restore packages
echo.
echo Restoring NuGet packages...
dotnet restore "%PROJECT%"

IF %ERRORLEVEL% NEQ 0 (
echo.
echo Restore FAILED
pause
exit /b %ERRORLEVEL%
)

echo.
echo ==========================================
echo Building Revit 2023
echo ==========================================

dotnet build "%PROJECT%" ^
-c Release-Revit2023 ^
-p:Platform=x64

IF %ERRORLEVEL% NEQ 0 (
echo Build FAILED for Revit 2023
pause
exit /b %ERRORLEVEL%
)

echo.
echo ==========================================
echo Building Revit 2024
echo ==========================================

dotnet build "%PROJECT%" ^
-c Release-Revit2024 ^
-p:Platform=x64

IF %ERRORLEVEL% NEQ 0 (
echo Build FAILED for Revit 2024
pause
exit /b %ERRORLEVEL%
)

echo.
echo ==========================================
echo Building Revit 2025
echo ==========================================

dotnet build "%PROJECT%" ^
-c Release-Revit2025 ^
-p:Platform=x64

IF %ERRORLEVEL% NEQ 0 (
echo Build FAILED for Revit 2025
pause
exit /b %ERRORLEVEL%
)

echo.
echo ==========================================
echo Building Revit 2026
echo ==========================================

dotnet build "%PROJECT%" ^
-c Release-Revit2026 ^
-p:Platform=x64

IF %ERRORLEVEL% NEQ 0 (
echo Build FAILED for Revit 2026
pause
exit /b %ERRORLEVEL%
)

echo.
echo ==========================================
echo BUILD SUCCESSFUL
echo ==========================================

echo.
echo Output Folder:
echo %ROOT_DIR%build\AdaptivePoints

pause
