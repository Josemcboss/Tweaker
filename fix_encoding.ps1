$path = "Tweaker/Data/TweaksDatabase.cs"
$content = [System.IO.File]::ReadAllText($path, [System.Text.Encoding]::UTF8)

$replacements = @{
    "Informacin" = "Información"
    "especfico" = "específico"
    "especfica" = "específica"
    "Aceleracin" = "Aceleración"
    "aceleracin" = "aceleración"
    "qu tan rpido" = "qué tan rápido"
    "fsicamente" = "físicamente"
    "Precisin" = "Precisión"
    "repeticin" = "repetición"
    "instantnea" = "instantánea"
    "rpida" = "rápida"
    "ms" = "más"
    "paginacin" = "paginación"
    "crtico" = "crítico"
    "fsica" = "física"
    "Optimizacin" = "Optimización"
    "Versin" = "Versión"
    "rpidos" = "rápidos"
    "mxima" = "máxima"
    "Crtico" = "Crítico"
    "grabacin" = "grabación"
    "vara" = "varía"
    "segn" = "según"
    "Energa" = "Energía"
    "batera" = "batería"
    "virtualizacin" = "virtualización"
    "prdidas" = "pérdidas"
    "est" = "está"
    "an" = "aún"
    "" = "•" # Replacing the broken bullet with a standard one
}

foreach ($key in $replacements.Keys) {
    $content = $content.Replace($key, $replacements[$key])
}

# Fix separators
$content = $content -replace "─+", "────────────────────────────────────────────────────────────────"

# Save with UTF-8 BOM
[System.IO.File]::WriteAllText($path, $content, [System.Text.Encoding]::UTF8)
Write-Host "Fixed encoding in TweaksDatabase.cs"
