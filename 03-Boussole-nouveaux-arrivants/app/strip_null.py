c = open("/tmp/b3/src/View.fs", "rb").read()
c = c.replace(b"\x00", b"")
c = c.replace(b"\r\n", b"\n").replace(b"\r", b"\n")
open("/tmp/b3/src/View.fs", "wb").write(c)
has_null = b"\x00" in c
print("null bytes after strip: " + str(has_null) + ", lines: " + str(c.count(b"\n")))