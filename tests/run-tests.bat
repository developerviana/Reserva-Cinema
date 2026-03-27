@echo off
REM Script para executar testes do ReservaCinema (Windows)
REM Uso: run-tests.bat [options]

setlocal enabledelayedexpansion

for %%I in ("%~dp0.") do set "PROJECT_ROOT=%%~dpI"

if "%1"=="" (
    echo Opcoes disponiveis:
    echo   run-tests all              - Executar todos os testes
    echo   run-tests domain           - Testes da camada Domain
    echo   run-tests application      - Testes da camada Application
    echo   run-tests api              - Testes da camada API
    echo   run-tests infrastructure   - Testes da camada Infrastructure
    echo   run-tests coverage         - Executar com cobertura
    exit /b 0
)

if "%1"=="all" (
    echo ================================
    echo Executando todos os testes
    echo ================================
    dotnet test "%PROJECT_ROOT%tests" --verbosity minimal
    if errorlevel 1 goto error
    goto success
)

if "%1"=="domain" (
    echo ================================
    echo Testes da camada Domain
    echo ================================
    dotnet test "%PROJECT_ROOT%tests\ReservaCinema.Domain.Tests\ReservaCinema.Domain.Tests.csproj" --verbosity minimal
    if errorlevel 1 goto error
    goto success
)

if "%1"=="application" (
    echo ================================
    echo Testes da camada Application
    echo ================================
    dotnet test "%PROJECT_ROOT%tests\ReservaCinema.Application.Tests\ReservaCinema.Application.Tests.csproj" --verbosity minimal
    if errorlevel 1 goto error
    goto success
)

if "%1"=="api" (
    echo ================================
    echo Testes da camada API
    echo ================================
    dotnet test "%PROJECT_ROOT%tests\ReservaCinema.API.Tests\ReservaCinema.API.Tests.csproj" --verbosity minimal
    if errorlevel 1 goto error
    goto success
)

if "%1"=="infrastructure" (
    echo ================================
    echo Testes da camada Infrastructure
    echo ================================
    dotnet test "%PROJECT_ROOT%tests\ReservaCinema.Infrastructure.Tests\ReservaCinema.Infrastructure.Tests.csproj" --verbosity minimal
    if errorlevel 1 goto error
    goto success
)

if "%1"=="coverage" (
    echo ================================
    echo Executando testes com cobertura
    echo ================================
    dotnet test "%PROJECT_ROOT%tests" ^
        /p:CollectCoverage=true ^
        /p:CoverletOutputFormat=html ^
        /p:CoverletOutput="%PROJECT_ROOT%tests\coverage\" ^
        --verbosity minimal
    if errorlevel 1 goto error
    echo.
    echo ✓ Relatorio de cobertura gerado em: tests\coverage\index.html
    goto success
)

echo Opcao desconhecida: %1
exit /b 1

:error
echo ✗ Erro ao executar testes
exit /b 1

:success
echo.
echo ✓ Testes executados com sucesso!
exit /b 0
