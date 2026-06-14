import os

# Read View.fs and fix BOM + CRLF
path = "/tmp/b3/src/View.fs"
content = open(path, "rb").read()

# Remove BOM if present
if content.startswith(b"\xef\xbb\xbf"):
    content = content[3:]

# Replace CRLF with LF
content = content.replace(b"\r\n", b"\n").replace(b"\r", b"\n")

# Check for ariaModal - must NOT be present
if b"ariaModal" in content:
    print("ERROR: ariaModal still present in View.fs!")
else:
    print("OK: ariaModal not found")

# Check for key extractions
if b"contenuHorizon" in content:
    print("OK: contenuHorizon let binding found")
else:
    print("WARNING: contenuHorizon not found - may still have if/else inside list")

if b"contenuCourrier" in content:
    print("OK: contenuCourrier let binding found")
else:
    print("WARNING: contenuCourrier not found")

if b"carteStatut" in content:
    print("OK: carteStatut let binding found")

# Write back clean version
open(path, "wb").write(content)
lines = content.count(b"\n")
print("View.fs cleaned: " + str(lines) + " lines, " + str(len(content)) + " bytes")

# Summary of all files
for fname in ["Domain.fs", "I18n.fs", "Storage.fs", "View.fs"]:
    fpath = "/tmp/b3/src/" + fname
    c = open(fpath, "rb").read()
    nl = c.count(b"\n")
    has_null = b"\x00" in c
    print(fname + ": " + str(nl) + " lines, null=" + str(has_null))