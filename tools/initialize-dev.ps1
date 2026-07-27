param(
    [Parameter(Mandatory = $true)]
    [string]$DecompiledProjectPath,
    [string]$ProjectPath,
    [switch]$Refresh
)

$ErrorActionPreference = "Stop"

function Resolve-Directory([string]$Path, [string]$Label) {
    if (-not (Test-Path -LiteralPath $Path -PathType Container)) {
        throw "$Label does not exist or is not a directory: $Path"
    }
    return (Resolve-Path -LiteralPath $Path).Path.TrimEnd('\', '/')
}

function Is-TextResource([string]$Path) {
    $extension = [System.IO.Path]::GetExtension($Path).ToLowerInvariant()
    return $extension -in @(
        ".cfg", ".cs", ".gd", ".gdshader", ".godot", ".json",
        ".material", ".shader", ".tres", ".tscn", ".txt"
    )
}

function Get-ResourceReferences([string]$Path) {
    if (-not (Is-TextResource $Path)) {
        return @()
    }
    try {
        $content = [System.IO.File]::ReadAllText($Path)
    }
    catch {
        Write-Warning "Could not read resource as text: $Path"
        return @()
    }
    $matches = [regex]::Matches($content, 'res://(?<path>[^"''\r\n\]\)\},]+)')
    return $matches | ForEach-Object {
        [System.Uri]::UnescapeDataString(
            $_.Groups["path"].Value.Trim().Replace('/', '\')
        )
    } | Where-Object {
        $_ -and -not [System.IO.Path]::IsPathRooted($_)
    } | Sort-Object -Unique
}

if ([string]::IsNullOrWhiteSpace($ProjectPath)) {
    $scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
    $ProjectPath = Split-Path -Parent $scriptDirectory
}

$projectRoot = Resolve-Directory $ProjectPath "Mod project"
$decompiledRoot = Resolve-Directory $DecompiledProjectPath "Decompiled StS2 project"
if ($projectRoot -eq $decompiledRoot) {
    throw "The mod project and decompiled project must be different directories."
}

foreach ($requiredPath in @("project.godot", "images\atlases\card_atlas_1.png", "src")) {
    if (-not (Test-Path -LiteralPath (Join-Path $decompiledRoot $requiredPath))) {
        throw "The supplied directory does not look like a complete decompiled StS2 project. Missing: $requiredPath"
    }
}

$manifestDirectory = Join-Path $projectRoot ".godot"
$manifestPath = Join-Path $manifestDirectory "eirene_dev_dependencies.json"
$knownDependencies = [System.Collections.Generic.HashSet[string]]::new(
    [System.StringComparer]::OrdinalIgnoreCase
)
$trackedFiles = [System.Collections.Generic.HashSet[string]]::new(
    [System.StringComparer]::OrdinalIgnoreCase
)
if (Test-Path -LiteralPath (Join-Path $projectRoot ".git")) {
    & git -C $projectRoot ls-files -z 2>$null |
        ForEach-Object {
            foreach ($trackedPath in ($_ -split "`0")) {
                if ($trackedPath) {
                    [void]$trackedFiles.Add($trackedPath.Replace('\', '/'))
                }
            }
        }
}
if (Test-Path -LiteralPath $manifestPath) {
    $oldManifest = Get-Content -Raw -LiteralPath $manifestPath | ConvertFrom-Json
    foreach ($relativePath in $oldManifest.files) {
        [void]$knownDependencies.Add([string]$relativePath)
    }
}

$ignoredDirectories = @(".git", ".godot", ".idea", "bin", "build", "obj") |
    ForEach-Object { Join-Path $projectRoot $_ }
$queue = [System.Collections.Generic.Queue[string]]::new()
Get-ChildItem -LiteralPath $projectRoot -Recurse -File | Where-Object {
    $fullName = $_.FullName
    -not ($ignoredDirectories | Where-Object {
        $fullName.StartsWith($_ + [System.IO.Path]::DirectorySeparatorChar)
    })
} | ForEach-Object {
    if (Is-TextResource $_.FullName) {
        $queue.Enqueue($_.FullName)
    }
}

$scanned = [System.Collections.Generic.HashSet[string]]::new(
    [System.StringComparer]::OrdinalIgnoreCase
)
$missingInSource = [System.Collections.Generic.HashSet[string]]::new(
    [System.StringComparer]::OrdinalIgnoreCase
)
$copiedCount = 0

while ($queue.Count -gt 0) {
    $resourceFile = $queue.Dequeue()
    if (-not $scanned.Add($resourceFile)) {
        continue
    }
    foreach ($reference in (Get-ResourceReferences $resourceFile)) {
        $destination = [System.IO.Path]::GetFullPath((Join-Path $projectRoot $reference))
        $source = [System.IO.Path]::GetFullPath((Join-Path $decompiledRoot $reference))
        if (-not $destination.StartsWith(
                $projectRoot + [System.IO.Path]::DirectorySeparatorChar,
                [System.StringComparison]::OrdinalIgnoreCase
            ) -or
            -not $source.StartsWith(
                $decompiledRoot + [System.IO.Path]::DirectorySeparatorChar,
                [System.StringComparison]::OrdinalIgnoreCase
            )) {
            Write-Warning "Skipped resource path outside the project: res://$reference"
            continue
        }
        $relativePath = $reference.Replace('\', '/')
        $isManagedDependency = $knownDependencies.Contains($relativePath)

        if (-not (Test-Path -LiteralPath $source -PathType Leaf)) {
            if (-not (Test-Path -LiteralPath $destination)) {
                [void]$missingInSource.Add($relativePath)
            }
            continue
        }

        $shouldCopy = -not (Test-Path -LiteralPath $destination -PathType Leaf)
        if ($Refresh -and $isManagedDependency -and
            -not $trackedFiles.Contains($relativePath)) {
            $shouldCopy = $true
        }
        if ($shouldCopy) {
            $destinationDirectory = Split-Path -Parent $destination
            [void](New-Item -ItemType Directory -Force -Path $destinationDirectory)
            Copy-Item -LiteralPath $source -Destination $destination -Force
            [void]$knownDependencies.Add($relativePath)
            $copiedCount++
        }
        if ((Test-Path -LiteralPath $destination -PathType Leaf) -and
            (Is-TextResource $destination)) {
            $queue.Enqueue($destination)
        }

        $sourceUid = "$source.uid"
        $destinationUid = "$destination.uid"
        $uidRelativePath = "$relativePath.uid"
        if ((Test-Path -LiteralPath $sourceUid -PathType Leaf) -and
            (-not (Test-Path -LiteralPath $destinationUid) -or
             ($Refresh -and $knownDependencies.Contains($uidRelativePath) -and
              -not $trackedFiles.Contains($uidRelativePath)))) {
            Copy-Item -LiteralPath $sourceUid -Destination $destinationUid -Force
            [void]$knownDependencies.Add($uidRelativePath)
            $copiedCount++
        }
    }
}

[void](New-Item -ItemType Directory -Force -Path $manifestDirectory)
$manifest = [ordered]@{
    decompiled_project = $decompiledRoot
    generated_at = (Get-Date).ToString("o")
    files = @($knownDependencies | Sort-Object)
}
$manifest | ConvertTo-Json -Depth 3 |
    Set-Content -LiteralPath $manifestPath -Encoding UTF8

$gitMarker = Join-Path $projectRoot ".git"
if (Test-Path -LiteralPath $gitMarker) {
    $excludePath = (& git -C $projectRoot rev-parse --path-format=absolute --git-path info/exclude 2>$null)
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($excludePath)) {
        Write-Warning "Could not locate Git's local exclude file. Copied dependencies may appear as untracked."
    } else {
        $excludePath = $excludePath.Trim()
        [void](New-Item -ItemType Directory -Force -Path (Split-Path -Parent $excludePath))
        try {
    $beginMarker = "# BEGIN Eirene local StS2 dependencies"
    $endMarker = "# END Eirene local StS2 dependencies"
    $existing = if (Test-Path -LiteralPath $excludePath) {
        Get-Content -Raw -LiteralPath $excludePath
    } else { "" }
    $escapedBeginMarker = [regex]::Escape($beginMarker)
    $escapedEndMarker = [regex]::Escape($endMarker)
    $pattern = "(?ms)^$escapedBeginMarker.*?^$escapedEndMarker\r?\n?"
    $cleaned = [regex]::Replace($existing, $pattern, "").TrimEnd()
    $generatedBlock = @(
        $beginMarker
        $knownDependencies | Sort-Object | ForEach-Object { "/$_" }
        $endMarker
    ) -join [Environment]::NewLine
    $newExclude = if ($cleaned) {
        "$cleaned$([Environment]::NewLine)$generatedBlock$([Environment]::NewLine)"
    } else {
        "$generatedBlock$([Environment]::NewLine)"
    }
            Set-Content -LiteralPath $excludePath -Value $newExclude -Encoding UTF8
        }
        catch {
            Write-Warning "Could not update Git's local exclude file: $($_.Exception.Message)"
            Write-Warning "Copied dependencies may appear as untracked, but initialization can continue."
        }
    }
}

Write-Host "Development dependencies are ready."
Write-Host "Copied or refreshed files: $copiedCount"
Write-Host "Dependency manifest: $manifestPath"
if ($missingInSource.Count -gt 0) {
    Write-Warning "$($missingInSource.Count) referenced resources were absent from both projects."
    $missingInSource | Sort-Object | Select-Object -First 20 | ForEach-Object {
        Write-Warning "  res://$_"
    }
    if ($missingInSource.Count -gt 20) {
        Write-Warning "  ...and $($missingInSource.Count - 20) more"
    }
}
Write-Host "Open this project in MegaDot: $projectRoot"
