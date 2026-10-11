#!/bin/sh
# Commits Scripts every way `git commit` takes them, in a scratch repo with .githooks installed,
# and fails unless each commit's scripts.json is up to date and the index and working tree match it.
#   sh Tools/ForDevelopers/scripts_json_test.sh
set -eu

src=$(cd "$(dirname "$0")/../.." && pwd)
repo=$(mktemp -d)
trap 'rm -rf "$repo"' EXIT
cd "$repo"

git init -q
git config user.name test
git config user.email test@example.com
git config core.hooksPath .githooks
mkdir -p .githooks Tools/ForDevelopers
cp "$src"/.githooks/* .githooks/
cp "$src/Tools/ForDevelopers/scripts_json.py" Tools/ForDevelopers/

write() { printf '/*\nname: %s\ndescription: test\ntags: test\n*/\n// %s\n' "$1" "$2" > "$1.cs"; }

failed=0
# git commit, failing the test if the hooks print anything: they sync quietly.
commit() {
  hook_output=$(git commit -q "$@" 2>&1)
  [ -z "$hook_output" ] || { printf 'FAIL git commit %s: the hooks printed\n%s\n' "$*" "$hook_output"; failed=1; }
}

verify() {
  index=$(mktemp)
  if ! out=$(GIT_INDEX_FILE=$index sh -c 'git read-tree HEAD && python3 Tools/ForDevelopers/scripts_json.py check' 2>&1); then
    printf 'FAIL %s: the commit is stale\n%s\n' "$1" "$out"
    failed=1
  elif [ -n "$(git status --porcelain -- scripts.json)" ]; then
    printf 'FAIL %s: scripts.json differs from the commit\n%s\n' "$1" "$(git status --short -- scripts.json)"
    failed=1
  else
    echo "ok   $1"
  fi
  rm -f "$index"
}

write A 1; write B 1; write C 1
echo '[]' > scripts.json
git add -A
commit -m init
verify 'git commit (first commit)'

write A 2
commit -m A A.cs
verify 'git commit A.cs'

write B 2
git add B.cs
commit -m B
verify 'git add B.cs && git commit, after git commit A.cs'

write C 2
commit -am C
verify 'git commit -a'

rm C.cs
commit -m C C.cs
verify 'git commit C.cs, deleting it'

write D 1
git add D.cs
commit -m D D.cs
verify 'git commit D.cs, adding it'

write A 3
git add A.cs
write A 4
commit -m A
verify 'git commit, with unstaged edits on top'

write B 3
git add B.cs
commit -m A A.cs
verify 'git commit A.cs, with B.cs staged'
commit -m B
verify 'git commit B.cs, staged before'

write B 4
git add B.cs
write B 5
write A 5
commit -m A A.cs
verify 'git commit A.cs, with B.cs staged and edited'
commit -m B
verify 'git commit B.cs, with unstaged edits on top'

write A 6
git add A.cs
write A 7
commit --amend -m 'A, amended'
verify 'git commit --amend, with unstaged edits on top'

# By hand, sync reads the index too, so it warns about Scripts whose changes aren't staged yet.
expect_warning() {
  out=$(python3 Tools/ForDevelopers/scripts_json.py sync "$@" 2>&1 >/dev/null)
  if [ "$(printf '%s\n' "$out" | grep -c '^  ')" -ne "$#" ] || ! printf '%s' "$out" | grep -q 'git add'; then
    printf 'FAIL sync %s: expected a warning naming each path\n%s\n' "$*" "$out"
    failed=1
  else
    echo "ok   sync $*, warning before git add"
  fi
}
write A 8; write E 1
expect_warning A.cs E.cs
git add A.cs E.cs
out=$(python3 Tools/ForDevelopers/scripts_json.py sync A.cs E.cs 2>&1)
[ -z "$out" ] || { printf 'FAIL sync after git add warned:\n%s\n' "$out"; failed=1; }
git add scripts.json
commit -m 'A and E, synced by hand' --no-verify
verify 'git add, sync by hand, git commit --no-verify'

exit $failed
