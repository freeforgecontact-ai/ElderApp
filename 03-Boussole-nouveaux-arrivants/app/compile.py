import subprocess
import os
import sys

# Set environment
env = dict(os.environ)
env["DOTNET_ROOT"] = os.path.expanduser("~/.dotnet")
env["PATH"] = os.path.expanduser("~/.dotnet") + ":" + os.path.expanduser("~/.dotnet/tools") + ":" + env.get("PATH", "")
env["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1"
env["DOTNET_NOLOGO"] = "1"

# Verify files
for f in ["Domain.fs", "I18n.fs", "Storage.fs", "View.fs", "Main.fs", "State.fs", "Contenu.fs", "Boussole.fsproj"]:
    path = "/tmp/b3/src/" + f
    lines = sum(1 for _ in open(path))
    print(f + ": " + str(lines) + " lines")

# Run fable
result = subprocess.run(
    ["dotnet", "fable", "src", "--noCache"],
    cwd="/tmp/b3",
    capture_output=True,
    text=True,
    env=env,
    timeout=120
)

output = result.stdout + result.stderr
# Strip ANSI
import re
output = re.sub(r'\x1b\[[0-9;]*m', '', output)

# Print relevant lines
for line in output.splitlines():
    lower = line.lower()
    if "error" in lower or "warning fsharp" in lower or "compilation" in lower:
        print(line)