# AI Core HMI - All-in-One Deployment Script
# Builds and starts all services with docker-compose

Write-Host "?? AI Core HMI - All-in-One Deployment" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

# Change to repository root
$scriptPath = Split-Path -Parent $PSCommandPath
$repoRoot = Split-Path -Parent $scriptPath
Set-Location $repoRoot

Write-Host "?? Building Docker images..." -ForegroundColor Yellow
docker-compose -f docker-compose.allinone.yml build

if ($LASTEXITCODE -ne 0) {
    Write-Host "? Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "?? Starting all services..." -ForegroundColor Yellow
docker-compose -f docker-compose.allinone.yml up -d

if ($LASTEXITCODE -ne 0) {
    Write-Host "? Startup failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "? All services started successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "?? Access Points:" -ForegroundColor Cyan
Write-Host "  • Web Editor:      http://localhost:8080" -ForegroundColor White
Write-Host "  • Runtime Viewer:  http://localhost:8088" -ForegroundColor White
Write-Host "  • OPC UA Server:   opc.tcp://localhost:14840" -ForegroundColor White
Write-Host "  • REST API:        http://localhost:14841" -ForegroundColor White
Write-Host "  • Ollama:          http://localhost:11434" -ForegroundColor White
Write-Host "  • PostgreSQL:      localhost:5432" -ForegroundColor White
Write-Host ""
Write-Host "?? View logs:    docker-compose -f docker-compose.allinone.yml logs -f" -ForegroundColor Gray
Write-Host "?? Stop all:     docker-compose -f docker-compose.allinone.yml down" -ForegroundColor Gray
