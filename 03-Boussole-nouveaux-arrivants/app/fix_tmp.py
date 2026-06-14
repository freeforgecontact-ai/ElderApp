MOUNT = "D:\Apps-Solidaires\03-Boussole-nouveaux-arrivants\app\src"

for name in ["Domain.fs", "I18n.fs", "Storage.fs"]:
    src = MOUNT + "/" + name
    dst = "/tmp/b3/src/" + name
    content = open(src, "rb").read()
    open(dst, "wb").write(content)
    nl = content.count(b"\n")
    sz = len(content)
    print(name + ": " + str(nl) + " lines, " + str(sz) + " bytes")