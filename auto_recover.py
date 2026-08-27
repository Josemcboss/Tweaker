import subprocess
import sys

sys.stdout.reconfigure(encoding='utf-8')

# Run git fsck --unreachable
res = subprocess.run(["git", "fsck", "--unreachable"], capture_output=True, text=True, encoding="utf-8")
if res.returncode != 0:
    print("git fsck failed:", res.stderr)
    sys.exit(1)

unreachable_blobs = []
for line in res.stdout.splitlines():
    if "unreachable blob" in line:
        parts = line.split()
        if len(parts) >= 3:
            unreachable_blobs.append(parts[2])

print(f"Found {len(unreachable_blobs)} unreachable blobs. Scanning...")

candidates = []
for h in unreachable_blobs:
    try:
        # We run git cat-file -p <hash> in raw bytes to avoid decoding exceptions initially
        cat_res = subprocess.run(["git", "cat-file", "-p", h], capture_output=True)
        if cat_res.returncode == 0:
            content_bytes = cat_res.stdout
            # Check for x:Name="DashboardPage"
            if b'x:Name="DashboardPage"' in content_bytes:
                content = content_bytes.decode('utf-8', errors='ignore')
                line_count = len(content.splitlines())
                candidates.append((h, line_count))
    except Exception as e:
        print(f"Error on {h}: {e}")

# Sort candidates by line count descending
candidates.sort(key=lambda x: x[1], reverse=True)
print("\nCandidates:")
for h, l in candidates:
    print(f"Hash: {h}, Lines: {l}")

if candidates:
    best_hash = candidates[0][0]
    print(f"\nRestoring best candidate ({best_hash}) to Tweaker/MainWindow.xaml...")
    cat_res = subprocess.run(["git", "cat-file", "-p", best_hash], capture_output=True)
    with open("Tweaker/MainWindow.xaml", "wb") as f:
        f.write(cat_res.stdout)
    print("Done!")
else:
    print("No candidates found.")
