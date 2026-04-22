Write-Host "Pushing Docker images to Docker Hub..." -ForegroundColor Cyan
$dockerHubUser = "clodprogea"
$imageName = "aicorehmi"
$tags = @("latest", "1.0")
foreach ($tag in $tags) {
    $targetImage = "${dockerHubUser}/${imageName}:${tag}"
    Write-Host "Tagging and pushing: $targetImage" -ForegroundColor Yellow
    docker tag hmi-allinone:latest $targetImage
    docker push $targetImage
}
Write-Host "Done! Images available at: docker pull ${dockerHubUser}/${imageName}:latest" -ForegroundColor Green
