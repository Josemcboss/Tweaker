import re

content = open('Tweaker/MainWindow.xaml.cs', 'r', encoding='utf-8').read()
matches = re.findall(r'_tweakHelper\.ExecuteTweak\s*\(\s*"([^"]+)"\s*,\s*"([^"]+)"\s*,\s*\(\)\s*=>\s*([^,\n\r]+)', content)
unique_tweaks = {}
for tweak_id, cat, func in matches:
    if tweak_id not in unique_tweaks:
        unique_tweaks[tweak_id] = (cat, func.strip())

with open('tweaks_found.txt', 'w', encoding='utf-8') as out:
    out.write(f'Total ExecuteTweak calls found: {len(matches)}\n')
    for k, v in sorted(unique_tweaks.items()):
        out.write(f'{k:30} -> {v[1]} ({v[0]})\n')
print('Done. Total unique:', len(unique_tweaks))
