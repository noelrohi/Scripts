# AGENTS.md

## scripts.json

Skua decides which Scripts are outdated from each `scripts.json` entry's size and SHA-256, so a commit that changes a `.cs` file carries its updated entry. Install the hooks once per clone and they do this for every committed `.cs` file:

```sh
git config core.hooksPath .githooks
```

CI runs `Tools/ForDevelopers/scripts_json.py check` on every pull request; its docstring and its failure message give the repair command.

## Pull requests

PRs target `noelrohi/Scripts` (base `Skua`); auqw/Scripts is upstream and gets none. After opening one, check the URL names `noelrohi/Scripts`.

## Upstream sync

Merging `upstream/Skua` (auqw/Scripts) into `Skua`:

1. On a `sync/upstream-<date>` branch, `git merge --no-ff upstream/Skua`.
2. Where the fork deliberately changed or deleted a Script, keep the fork's side and fold in upstream's new fixes.
3. Resolve `scripts.json` by regenerating it: `git checkout --ours scripts.json && git add scripts.json && python3 Tools/ForDevelopers/scripts_json.py sync --all`.
4. Before pushing: `scripts_json.py check` passes, and `skua scripts check <path>` passes for every `.cs` the merge changes.
5. Title the PR `Merge upstream auqw/Scripts Skua (<N> commits)` and merge it with a merge commit, so the next sync finds the right merge base. #40 and #42 show the body.

## Testing a branch on a Skua Engine

Push the branch, then point the Engine's Script Source at it and sync:

```sh
skua scripts source noelrohi/Scripts@<branch>
skua scripts update
```

Set it back with `skua scripts source --default` (`noelrohi/Scripts@Skua`) when done. [noelrohi/Skua#215](https://github.com/noelrohi/Skua/issues/215) adds `skua scripts check <path>` and hash-verified updates.
