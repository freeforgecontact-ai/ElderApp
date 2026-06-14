state = open("/tmp/b3/src/State.fs", "r").read()
# Fix the duplicate "| _" line
state = state.replace("            | _\n            | _                   -> Some t", "            | _                   -> Some t")
open("/tmp/b3/src/State.fs", "w").write(state)
lines = state.count("\n")
print("State.fs cleaned: " + str(lines) + " lines")
# Print the ChoisirCourrier section
idx = state.find("ChoisirCourrier")
print("Context: " + state[idx:idx+200])