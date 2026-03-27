#!/bin/bash

# Script para executar testes do ReservaCinema
# Uso: ./run-tests.sh [options]

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Funções
print_header() {
    echo -e "${YELLOW}================================${NC}"
    echo -e "${YELLOW}$1${NC}"
    echo -e "${YELLOW}================================${NC}"
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

# Executar testes
run_tests() {
    local test_project=$1
    local filter=$2
    
    if [ -z "$filter" ]; then
        print_header "Executando testes: $test_project"
        dotnet test "$test_project" --verbosity minimal
    else
        print_header "Executando testes: $test_project (filtro: $filter)"
        dotnet test "$test_project" --filter "$filter" --verbosity minimal
    fi
}

# Menu
if [ $# -eq 0 ]; then
    echo "Opções disponíveis:"
    echo "  ./run-tests.sh all              - Executar todos os testes"
    echo "  ./run-tests.sh domain           - Testes da camada Domain"
    echo "  ./run-tests.sh application      - Testes da camada Application"
    echo "  ./run-tests.sh api              - Testes da camada API"
    echo "  ./run-tests.sh infrastructure   - Testes da camada Infrastructure"
    echo "  ./run-tests.sh coverage         - Executar com cobertura"
    exit 0
fi

case "$1" in
    all)
        print_header "Executando todos os testes"
        dotnet test "$PROJECT_ROOT/tests" --verbosity minimal
        print_success "Todos os testes executados com sucesso!"
        ;;
    domain)
        run_tests "$PROJECT_ROOT/tests/ReservaCinema.Domain.Tests/ReservaCinema.Domain.Tests.csproj"
        ;;
    application)
        run_tests "$PROJECT_ROOT/tests/ReservaCinema.Application.Tests/ReservaCinema.Application.Tests.csproj"
        ;;
    api)
        run_tests "$PROJECT_ROOT/tests/ReservaCinema.API.Tests/ReservaCinema.API.Tests.csproj"
        ;;
    infrastructure)
        run_tests "$PROJECT_ROOT/tests/ReservaCinema.Infrastructure.Tests/ReservaCinema.Infrastructure.Tests.csproj"
        ;;
    coverage)
        print_header "Executando testes com cobertura"
        dotnet test "$PROJECT_ROOT/tests" \
            /p:CollectCoverage=true \
            /p:CoverletOutputFormat=html \
            /p:CoverletOutput="$PROJECT_ROOT/tests/coverage/" \
            --verbosity minimal
        print_success "Relatório de cobertura gerado em: tests/coverage/index.html"
        ;;
    *)
        print_error "Opção desconhecida: $1"
        exit 1
        ;;
esac
