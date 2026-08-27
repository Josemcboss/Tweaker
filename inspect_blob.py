import subprocess
import re

# Get the contents of the hash
h = "3d2c4ad9d8c45251af14c26f3140e0cb21b2ec89"
res = subprocess.run(["git", "cat-file", "-p", h], capture_output=True)
content = res.stdout.decode('utf-8', errors='ignore')

# Find all occurrences of ScrollViewer x:Name="..."
pages = re.findall(r'<ScrollViewer\s+x:Name="([^"]+)"', content)
print("Pages in this blob:")
for p in pages:
    print(f"- {p}")

# Also print lines containing "AUTO-MAINTENANCE" (case insensitive)
for i, line in enumerate(content.splitlines()):
    if "maintenance" in line.lower() or "auto-maintenance" in line.lower() or "game mode" in line.lower():
        if "Comment" not in line and "<!--" in line:
            print(f"Line {i+1}: {line.strip()}")
