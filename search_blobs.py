import subprocess
import sys

res = subprocess.run(["git", "fsck", "--unreachable"], capture_output=True, text=True, errors="ignore")
objects = []
for line in res.stdout.splitlines():
    parts = line.split()
    if len(parts) >= 3:
        objects.append((parts[1], parts[2]))

print(f"Scanning {len(objects)} unreachable objects...")
found = []
for otype, sha in objects:
    if otype in ("blob", "commit"):
        cat_res = subprocess.run(["git", "cat-file", "-p", sha], capture_output=True)
        if b"AUTO-MAINTENANCE" in cat_res.stdout or b"PerformancePage" in cat_res.stdout:
            print(f"FOUND in {otype} {sha}!")
            found.append(sha)

if not found:
    print("Not found in any unreachable object.")
