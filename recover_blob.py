import subprocess

hashes = [
    "34803af5c72ed04e46815a4c1484d053e27267ac",
    "4da0e975deb773f9d3c3a4ff3426529c37cee359",
    "62d0321fe3514b5833731d7987a4650b846b8707",
    "63801c495df25c14549b65ae0d6839c52522ff36",
    "ebb2b0697e8307a0748c46d87a0713c6eb3d810f",
    "892367dc75529f5cec9dd38c04dfe4f43376f728",
    "9d538d13e70f74b7d06e6c33a7f5f412080ec6f0",
    "6db54066bbafef5930db0a58f1a0cf4ddb9a72fb",
    "ac35e8851b17d669cc21d7daf96efe81d94aa7fb",
    "2ef629d866096b797410500b71123d022d100598",
    "3de6f8cd2a307c63a81f06bdf483eba856d592ea",
    "7596d84722fe08cbf50efcfa5ccc60abc249059f",
    "d406032f6bf412d36edb31053808778bfc2916d0",
    "88e7762c7a21a13f932e73e696689255fa4cd92f",
    "9c189f9169315b554cb23cf71509bd26f739b5f6",
    "7e99a4ff277656f9e85296fd1c07cedb59d0a512",
    "4aaa59f7239d7ccff03f839209d2f0a06c574bcc",
    "545a6f5640a1f6ba1caebdfb54e8cf9e2d6e54f9",
    "f82a400e0b9edf94efcf84c35970c9c8faf3cd95",
    "52dbb0ccaa3dfb5c23f7402a235bd31c2228d610",
    "735b9befe854fd3cf33fe69bed409d38cdc9243a",
    "89db4016181b98b526b0028abfd146973253095d",
    "feab5cd19c285001d966f52df0fb31099d3889ea",
    "3d2c4ad9d8c45251af14c26f3140e0cb21b2ec89",
    "408c49f32b753ddb33c9fc32371c80893f0d2cb6",
    "d3ec4337690bdee070fc1b777f1fda6274c3670c",
    "638d28067ca1a9bf66cc1a159b75a61e63ba0da1",
    "a90e752b9eee40a27824110be89132966d6e0ea7",
    "47ef7ddf3d5b3cc653f0a649716256b67a77c355",
    "523f42d903d632515f7b0e851532d55db3f328d4",
    "53df683b6f91ab028e703f0e74595b66c73111c1",
    "91cf64bb26441e5cde8ee3c48b324e0f916cc86e"
]

results = []
for h in hashes:
    try:
        res = subprocess.run(["git", "cat-file", "-p", h], capture_output=True, text=True, encoding="utf-8")
        if res.returncode == 0:
            content = res.stdout
            if "x:Name=\"DashboardPage\"" in content:
                results.append((h, len(content.splitlines())))
    except Exception as e:
        pass

# Print sorted results by line count descending
results.sort(key=lambda x: x[1], reverse=True)
for h, l in results:
    print(f"Hash: {h}, Lines: {l}")
