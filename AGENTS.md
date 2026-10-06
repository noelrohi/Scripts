# AGENTS.md

## scripts.json

Skua decides which Scripts are outdated from each `scripts.json` entry's size and SHA-256, so a commit that changes a `.cs` file carries its updated entry. Install the pre-commit hook once per clone and it does this for every staged `.cs` file:

```sh
git config core.hooksPath .githooks
```

Without the hook, or to repair a stale file, run `python3 Tools/ForDevelopers/scripts_json.py sync --all`. CI runs `scripts_json.py check` on every pull request.

## Testing a branch on a Skua Engine

Push the branch, then point the Engine's Script Source at it and sync:

```sh
skua scripts source noelrohi/Scripts@<branch>
skua scripts update
```

Set it back with `skua scripts source --default` (`noelrohi/Scripts@Skua`) when done.

Coming once [noelrohi/Skua#215](https://github.com/noelrohi/Skua/issues/215) merges:

- `skua scripts check <path>` compiles a Script and its includes on no Engine and reports errors with file and line.
- `skua scripts update` can hash every local Script against `scripts.json` and re-download each one that differs, so a branch's edits reach the Engine even after earlier syncs.
