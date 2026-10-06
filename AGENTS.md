# AGENTS.md

## scripts.json

Skua decides which Scripts are outdated from each `scripts.json` entry's size and SHA-256, so a commit that changes a `.cs` file carries its updated entry. Install the hooks once per clone and they do this for every committed `.cs` file:

```sh
git config core.hooksPath .githooks
```

CI runs `Tools/ForDevelopers/scripts_json.py check` on every pull request; its docstring and its failure message give the repair command.

## Testing a branch on a Skua Engine

Push the branch, then point the Engine's Script Source at it and sync:

```sh
skua scripts source noelrohi/Scripts@<branch>
skua scripts update
```

Set it back with `skua scripts source --default` (`noelrohi/Scripts@Skua`) when done. [noelrohi/Skua#215](https://github.com/noelrohi/Skua/issues/215) adds `skua scripts check <path>` and hash-verified updates.
