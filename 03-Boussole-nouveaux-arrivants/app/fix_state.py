state = open("/tmp/b3/src/State.fs", "r").read()
# The file ends with "| _  " - we need to complete the ChoisirCourrier case and add ReseauChange
# Find where the truncation happened
truncated_end = "| _  "
if state.rstrip().endswith("| _"):
    # Remove the incomplete line and add the proper completion
    state = state.rstrip()
    # Remove the incomplete "| _  " at end
    while state.endswith("|") or state.endswith("_ ") or state.endswith("_  "):
        state = state.rstrip()
        if state.endswith("| _"):
            state = state[:-3].rstrip()
            break
    
    completion = """
            | _                   -> Some t
        { model with CourrierChoisi = nouveau }, Cmd.none

    | ReseauChange b ->
        { model with EnLigne = b }, Cmd.none
"""
    state = state + completion
    open("/tmp/b3/src/State.fs", "w").write(state)
    lines = state.count("\n")
    print("State.fs fixed: " + str(lines) + " lines")
else:
    print("ERROR: unexpected end of State.fs")
    print("Last 100 chars: " + repr(state[-100:]))