import os
import sys

search_terms = [b"AUTO-MAINTENANCE", b"PerformancePage", b"BtnNavPerformance"]
root_dir = "Tweaker"

print("Searching binary/compiled files in Tweaker directory...")
found = False
for dirpath, dirnames, filenames in os.walk(root_dir):
    for filename in filenames:
        filepath = os.path.join(dirpath, filename)
        # Skip source files that we know don't have it or have been restored
        if filename.endswith((".cs", ".xaml", ".xaml.backup")):
            continue
        try:
            with open(filepath, "rb") as f:
                content = f.read()
                for term in search_terms:
                    if term in content:
                        print(f"FOUND term {term.decode()} in {filepath}!")
                        found = True
        except Exception as e:
            pass

if not found:
    print("No binary/compiled files contained the search terms.")
