import re
import sys

sys.stdout.reconfigure(encoding='utf-8')

with open('MainWindow.xaml', 'r', encoding='utf-8') as f:
    lines = f.readlines()

for i, line in enumerate(lines):
    if 773 <= i < 1399:
        comment_match = re.search(r'<!--(.*?)-->', line)
        if comment_match:
            print(f"Line {i+1}: {comment_match.group(1).strip()}")
