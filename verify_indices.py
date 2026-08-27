with open("Tweaker/MainWindow.xaml", "r", encoding="utf-8") as f:
    lines = f.readlines()

print(f"Total lines: {len(lines)}")
# Let's inspect some lines to verify they match our decompiled lines
print("Line 498:", lines[498-1].strip())
print("Line 499:", lines[499-1].strip())
print("Line 504:", lines[504-1].strip())
print("Line 519:", lines[519-1].strip())
print("Line 564:", lines[564-1].strip())
print("Line 606:", lines[606-1].strip())
print("Line 637:", lines[637-1].strip())
print("Line 675:", lines[675-1].strip())
print("Line 694:", lines[694-1].strip())
print("Line 799:", lines[799-1].strip())
print("Line 807:", lines[807-1].strip())
print("Line 815:", lines[815-1].strip())
print("Line 869:", lines[869-1].strip())
print("Line 870:", lines[870-1].strip())
