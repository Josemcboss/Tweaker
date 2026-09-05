import re

content = open('Tweaker/Services/TweakDispatcher.cs', 'r', encoding='utf-8').read()
# match Register("...", ...);
blocks = re.findall(r'Register\(\s*"([^"]+)"\s*,\s*new TweakActions\s*\{([^}]+)\}(?:,\s*([^\)]+))?\);', content)

print(f'Total Register blocks found: {len(blocks)}')
with open('dispatcher_registered.txt', 'w', encoding='utf-8') as f:
    for can_id, body, aliases in blocks:
        has_get_state = 'GetState' in body
        f.write(f'ID: {can_id:30} | HasGetState: {str(has_get_state):5} | Aliases: {aliases.strip() if aliases else "None"}\n')
print('Wrote to dispatcher_registered.txt')
