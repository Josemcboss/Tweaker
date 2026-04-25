# Use character codes to avoid encoding issues in the script itself
$replacements = @{
    [char]0x26A1 = "&#x26A1;" # ⚡
    ([char]0xD83D + [char]0xDE80) = "&#x1F680;" # 🚀
    [char]0x2705 = "&#x2705;" # ✅
    [char]0x26A0 = "&#x26A0;&#xFE0F;" # ⚠️ (handling basic char)
    ([char]0xD83D + [char]0xDCAF) = "&#x1F3AF;" # 🎯
    ([char]0xD83D + [char]0xDCBE) = "&#x1F4BE;" # 💾
    ([char]0xD83D + [char]0xDD0D) = "&#x1F50D;" # 🔍
    ([char]0xD83C + [char]0xDFC6) = "&#x1F3C6;" # 🏆
    ([char]0xD83E + [char]0xDDF9) = "&#x1F9F9;" # 🧹
    ([char]0xD83D + [char]0xDCE6) = "&#x1F4E6;" # 📦
    ([char]0xD83D + [char]0xDCBB) = "&#x1F4BB;" # 💻
    ([char]0xD83D + [char]0xDEE1) = "&#x1F6E1;&#xFE0F;" # 🛡️
    ([char]0xD83D + [char]0xDD04) = "&#x1F504;" # 🔄
    ([char]0xD83C + [char]0xDF0A) = "&#x1F30A;" # 🌊
    [char]0x2696 = "&#x2696;&#xFE0F;" # ⚖️
    ([char]0xD83D + [char]0xDD0C) = "&#x1F50C;" # 🔌
    ([char]0xD83D + [char]0xDDDA) = "&#x1F5DA;&#xFE0F;" # 🖥️
    ([char]0xD83C + [char]0xDF10) = "&#x1F310;" # 🌐
    ([char]0xD83D + [char]0xDED1) = "&#x1F6D1;" # 🛑
    ([char]0xD83C + [char]0xDFAE) = "&#x1F3AE;" # 🎮
    ([char]0xD83D + [char]0xDCD6) = "&#x1F4D6;" # 📖
}

$files = Get-ChildItem -Path "Tweaker" -Filter "*.xaml" -Recurse

foreach ($file in $files) {
    $content = [System.IO.File]::ReadAllText($file.FullName, [System.Text.Encoding]::UTF8)
    $modified = $false
    
    foreach ($emoji in $replacements.Keys) {
        if ($content.Contains($emoji)) {
            $content = $content.Replace($emoji, $replacements[$emoji])
            $modified = $true
        }
    }
    
    if ($modified) {
        [System.IO.File]::WriteAllText($file.FullName, $content, [System.Text.Encoding]::UTF8)
        Write-Host "Updated icons in $($file.Name)"
    }
}

# C# Replacements
$csReplacements = @{
    [char]0x26A1 = "\u26A1"
    ([char]0xD83D + [char]0xDE80) = "\U0001F680"
    [char]0x2705 = "\u2705"
    [char]0x274C = "\u274C" # ❌
    ([char]0xD83D + [char]0xDD0D) = "\U0001F50D"
}

$csFiles = Get-ChildItem -Path "Tweaker" -Filter "*.cs" -Recurse

foreach ($file in $csFiles) {
    $content = [System.IO.File]::ReadAllText($file.FullName, [System.Text.Encoding]::UTF8)
    $modified = $false
    
    foreach ($emoji in $csReplacements.Keys) {
        if ($content.Contains($emoji)) {
            $content = $content.Replace($emoji, $csReplacements[$emoji])
            $modified = $true
        }
    }
    
    if ($modified) {
        [System.IO.File]::WriteAllText($file.FullName, $content, [System.Text.Encoding]::UTF8)
        Write-Host "Updated icons in $($file.Name)"
    }
}
