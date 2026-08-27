import sys

with open('MainWindow.xaml', 'r', encoding='utf-8') as f:
    content = f.read()

start_idx = content.find('x:Name="DashboardPage"')
if start_idx == -1:
    print("Not found")
    sys.exit(1)

# Find the tag start (less than sign before the name)
tag_start = content.rfind('<ScrollViewer', 0, start_idx)

# Now count depth of <ScrollViewer and </ScrollViewer>
depth = 0
pos = tag_start
end_idx = -1
while pos < len(content):
    if content.startswith('<ScrollViewer', pos):
        depth += 1
        pos += len('<ScrollViewer')
    elif content.startswith('</ScrollViewer>', pos):
        depth -= 1
        pos += len('</ScrollViewer>')
        if depth == 0:
            end_idx = pos
            break
    else:
        pos += 1

if end_idx != -1:
    # count lines
    start_line = content.count('\n', 0, tag_start) + 1
    end_line = content.count('\n', 0, end_idx) + 1
    print(f"StartLine: {start_line}, EndLine: {end_line}")
else:
    print("Matching </ScrollViewer> not found")
