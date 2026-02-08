# ?? Verificación de Emojis en el Código
# Este script busca y reporta todos los emojis usados en el código

Write-Host "???????????????????????????????????????" -ForegroundColor Cyan
Write-Host "   ?? VERIFICACIÓN DE EMOJIS" -ForegroundColor Green
Write-Host "???????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

$files = Get-ChildItem -Path "." -Recurse -Include "*.cs","*.xaml" -File

$emojiPattern = '[^\x00-\x7F]+'  # Buscar caracteres no ASCII

$results = @()

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $lines = Get-Content $file.FullName
    
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match $emojiPattern) {
            # Extraer el emoji
            $matches = [regex]::Matches($lines[$i], $emojiPattern)
            foreach ($match in $matches) {
                $results += [PSCustomObject]@{
                    File = $file.Name
                    Line = $i + 1
                    Emoji = $match.Value
                    Context = $lines[$i].Trim()
                }
            }
        }
    }
}

Write-Host "?? Emojis encontrados: $($results.Count)" -ForegroundColor Yellow
Write-Host ""

# Agrupar por emoji
$emojiGroups = $results | Group-Object Emoji

foreach ($group in $emojiGroups | Sort-Object Count -Descending) {
    Write-Host "?????????????????????????????????????" -ForegroundColor Gray
    Write-Host "Emoji: $($group.Name)  |  Usos: $($group.Count)" -ForegroundColor Cyan
    Write-Host ""
    
    foreach ($result in $group.Group | Select-Object -First 5) {
        Write-Host "  ?? $($result.File):$($result.Line)" -ForegroundColor White
        Write-Host "     $($result.Context.Substring(0, [Math]::Min(80, $result.Context.Length)))..." -ForegroundColor Gray
        Write-Host ""
    }
    
    if ($group.Count -gt 5) {
        Write-Host "     ... y $($group.Count - 5) más" -ForegroundColor DarkGray
        Write-Host ""
    }
}

Write-Host "???????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Resumen
Write-Host "?? RESUMEN" -ForegroundColor Yellow
Write-Host ""
Write-Host "Total de archivos analizados: $($files.Count)" -ForegroundColor White
Write-Host "Archivos con emojis: $($results | Select-Object -Unique File | Measure-Object | Select-Object -ExpandProperty Count)" -ForegroundColor White
Write-Host "Total de emojis encontrados: $($results.Count)" -ForegroundColor White
Write-Host "Emojis únicos: $($emojiGroups.Count)" -ForegroundColor White
Write-Host ""

# Recomendaciones
Write-Host "?? RECOMENDACIONES" -ForegroundColor Green
Write-Host ""
Write-Host "1. Los emojis en strings pueden causar problemas de encoding" -ForegroundColor Gray
Write-Host "2. Usa caracteres ASCII simples en titles por defecto" -ForegroundColor Gray
Write-Host "3. Agrega emojis programáticamente con código Unicode" -ForegroundColor Gray
Write-Host "4. Considera usar símbolos ASCII: [?] [X] [!] [i]" -ForegroundColor Gray
Write-Host ""

Write-Host "Ejemplo de corrección:" -ForegroundColor Yellow
Write-Host '  Antes: public void ShowSuccess(string message, string title = "? Éxito")' -ForegroundColor Red
Write-Host '  Después: public void ShowSuccess(string message, string title = "Tweak Activado")' -ForegroundColor Green
Write-Host '           ShowNotification(message, "? " + title, ...)' -ForegroundColor Green
Write-Host ""
