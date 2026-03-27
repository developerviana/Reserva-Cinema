#!/usr/bin/env pwsh

# Script para gerar visualização da estrutura de testes
# Uso: .\show-structure.ps1

Write-Host "`n╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║  📊 ESTRUTURA DE TESTES - RESERVACINEMA               ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan

$structure = @"
tests/
│
├── 📂 ReservaCinema.Domain.Tests/
│   ├── 📂 Entities/
│   │   └── 🧪 SessionTests.cs
│   └── 📄 ReservaCinema.Domain.Tests.csproj
│
├── 📂 ReservaCinema.Application.Tests/
│   ├── 📂 DTOs/
│   │   └── 🧪 CreateSessionRequestTests.cs
│   ├── 📂 UseCases/
│   ├── 📂 Services/
│   ├── 📂 Validators/
│   └── 📄 ReservaCinema.Application.Tests.csproj
│
├── 📂 ReservaCinema.API.Tests/
│   ├── 📂 Unit/
│   │   ├── 📂 Controllers/
│   │   ├── 📂 DTOs/
│   │   │   └── 🧪 CreateSessionRequestDTOTests.cs
│   │   └── ...
│   ├── 📂 Integration/
│   │   ├── 📂 Sessions/
│   │   │   └── 🧪 CreateSessionIntegrationTests.cs
│   │   └── ...
│   ├── 📂 Fixtures/
│   └── 📄 ReservaCinema.API.Tests.csproj
│
├── 📂 ReservaCinema.Infrastructure.Tests/
│   ├── 📂 Repositories/
│   │   └── 🧪 SessionRepositoryTests.cs
│   ├── 📂 Cache/
│   ├── 📂 Messaging/
│   └── 📄 ReservaCinema.Infrastructure.Tests.csproj
│
├── 📂 Shared/
│   ├── 📂 Fixtures/
│   │   └── 📄 BaseFixture.cs
│   ├── 📂 Builders/
│   │   ├── 📄 SessionBuilder.cs
│   │   └── 📄 CreateSessionRequestBuilder.cs
│   ├── 📂 Mocks/
│   │   └── 📄 RepositoryMockFactory.cs
│   ├── 📂 Constants/
│   │   └── 📄 TestDataConstants.cs
│   ├── 📂 Extensions/
│   │   └── 📄 TestExtensions.cs
│   └── 📄 GlobalUsings.cs
│
├── 📚 Documentação
│   ├── 📋 INDEX.md                (COMECE AQUI)
│   ├── 📋 README.md               (Visão geral)
│   ├── 📋 STRUCTURE_OVERVIEW.md   (Visão estrutural)
│   ├── 📋 CONTRIBUTING.md         (Como contribuir)
│   └── 📋 TEST_TEMPLATE.md        (Template padrão)
│
├── 🔧 Configuração
│   ├── 📄 run-tests.bat           (Windows)
│   ├── 📄 run-tests.sh            (Linux/Mac)
│   ├── 📄 coverletSettings.json   (Cobertura)
│   ├── 📄 xunit.runner.json       (xUnit config)
│   └── 📄 .gitignore              (Git)
│
└── 📄 Este arquivo (show-structure.ps1)

"@

Write-Host $structure -ForegroundColor White

Write-Host "`n╔════════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║  ✅ TOTALIZANDO:                                       ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════════════════════════╝`n" -ForegroundColor Green

$stats = @{
    "Projetos de Teste" = 4
    "Builders" = 2
    "Testes Exemplo" = 6
    "Documentos" = 5
    "Scripts" = 2
    "Constantes/Extensões" = 2
}

$stats.GetEnumerator() | ForEach-Object {
    Write-Host "  📊 $($_.Key): " -NoNewline -ForegroundColor Cyan
    Write-Host "$($_.Value)" -ForegroundColor Yellow
}

Write-Host "`n╔════════════════════════════════════════════════════════════╗" -ForegroundColor Magenta
Write-Host "║  🚀 PRÓXIMOS PASSOS:                                   ║" -ForegroundColor Magenta
Write-Host "╚════════════════════════════════════════════════════════════╝`n" -ForegroundColor Magenta

$steps = @(
    "1. Leia 📋 INDEX.md para visão geral",
    "2. Consulte CONTRIBUTING.md para adicionar novos testes",
    "3. Use os BUILDERS em Shared/ para criar dados",
    "4. Execute: run-tests.bat all (ou .sh no Linux/Mac)",
    "5. Mantenha cobertura acima de 80%",
    "6. Siga o padrão AAA (Arrange, Act, Assert)"
)

$steps | ForEach-Object {
    Write-Host "  $_ " -ForegroundColor White
}

Write-Host "`n╔════════════════════════════════════════════════════════════╗" -ForegroundColor Blue
Write-Host "║  📚 DOCUMENTAÇÃO RÁPIDA:                               ║" -ForegroundColor Blue
Write-Host "╚════════════════════════════════════════════════════════════╝`n" -ForegroundColor Blue

$docs = @{
    "INDEX.md" = "Comece aqui!"
    "README.md" = "Visão geral detalhada"
    "CONTRIBUTING.md" = "Como contribuir"
    "TEST_TEMPLATE.md" = "Template padrão"
    "STRUCTURE_OVERVIEW.md" = "Visão visual"
}

$docs.GetEnumerator() | ForEach-Object {
    Write-Host "  📄 $($_.Key) " -NoNewline -ForegroundColor Yellow
    Write-Host "→ $($_.Value)" -ForegroundColor White
}

Write-Host "`n╔════════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║  ✨ ESTRUTURA PRONTA PARA USO!                         ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════════════════════════╝`n" -ForegroundColor Green
