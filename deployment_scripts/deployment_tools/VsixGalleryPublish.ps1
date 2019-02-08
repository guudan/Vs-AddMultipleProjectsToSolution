param(
    [string]$VsixPath
)
Write-Host "Running powershell to publish VSIX to Vsix Gallery"
Write-Host "Vsix Path: $VsixPath"

# Get publish script
(new-object Net.WebClient).DownloadString("https://raw.github.com/madskristensen/ExtensionScripts/master/AppVeyor/vsix.ps1") | iex
# Perform publish
Vsix-PublishToGallery "$VsixPath"