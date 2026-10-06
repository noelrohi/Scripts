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
git commit -qm init
verify 'git commit (first commit)'

write A 2
git commit -qm A A.cs
verify 'git commit A.cs'

write B 2
git add B.cs
git commit -qm B
verify 'git add B.cs && git commit, after git commit A.cs'

write C 2
git commit -qam C
verify 'git commit -a'

rm C.cs
git commit -qm C C.cs
verify 'git commit C.cs, deleting it'

write D 1
git add D.cs
git commit -qm D D.cs
verify 'git commit D.cs, adding it'

write A 3
git add A.cs
write A 4
git commit -qm A
verify 'git commit, with unstaged edits on top'

write B 3
git add B.cs
git commit -qm A A.cs
verify 'git commit A.cs, with B.cs staged'
git commit -qm B
verify 'git commit B.cs, staged before'

exit $failed
