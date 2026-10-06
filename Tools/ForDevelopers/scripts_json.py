#!/usr/bin/env python3
"""Keeps scripts.json in step with the Scripts, as SkuaScriptsGenerator would write it.

Skua serves a Script only when its size and SHA-256 match its scripts.json entry, so an
edited Script needs its entry rewritten in the same commit. Files are read from the git
index, i.e. what the commit will hold, not the working tree.

  scripts_json.py sync PATH...   rewrite the entries of these Scripts, in the index and the
                                 working tree; a deleted Script, or one without a header,
                                 loses its entry
  scripts_json.py sync --all     the same for every Script
  scripts_json.py check          fail when an entry is stale, or a Script has none

The pre-commit hook in .githooks runs `sync` on the staged .cs files.
"""
import hashlib
import json
import os
import re
import subprocess
import sys

SCRIPTS_JSON = "scripts.json"
RAW_URL = "https://raw.githubusercontent.com/auqw/Scripts/Skua/"


def git(*args, stdin=None):
    return subprocess.run(["git", *args], input=stdin, capture_output=True, check=True).stdout


def staged_blobs(paths=None):
    """Path -> blob id of the staged .cs files, or of the given paths that are staged."""
    out = git("--literal-pathspecs", "ls-files", "-s", "-z", "--", *(paths or []))
    blobs = {}
    for record in out.decode().split("\0"):
        if record:
            meta, path = record.split("\t", 1)
            if path.endswith(".cs"):
                blobs[path] = meta.split()[1]
    return blobs


def read_blobs(ids):
    """Blob id -> contents, read in one `git cat-file --batch`."""
    out = git("cat-file", "--batch", stdin="".join(i + "\n" for i in ids).encode())
    contents, pos = {}, 0
    for i in ids:
        header_end = out.index(b"\n", pos)
        size = int(out[pos:header_end].split()[2])
        contents[i] = out[header_end + 1 : header_end + 1 + size]
        pos = header_end + 1 + size + 1
    return contents


def entry(path, data):
    """The scripts.json entry SkuaScriptsJsonWriter writes for a Script, or None without a /* header."""
    # File.ReadLines drops a BOM and splits on \r\n, \r and \n.
    lines = re.split(r"\r\n|\r|\n", data.decode("utf-8-sig", errors="replace"))
    if not lines[0].startswith("/*"):
        return None
    name = description = tags = None
    for line in lines[1:]:
        if line.startswith("*/"):
            break
        parts = line.split(":", 1)
        if len(parts) < 2:
            continue
        key, value = parts[0].strip(), parts[1].strip()
        if key == "name":
            name = value
        elif key == "description":
            description = value
        elif key == "tags":
            tags = [t if all(c.isupper() for c in t) else t.lower() for t in (t.strip() for t in value.split(","))]
    # The hash is of the UTF-8 text without zero-width spaces and BOMs.
    text = re.sub("[​﻿]", "", data.decode("utf-8", errors="replace")).encode()
    return {
        "name": name,
        "description": description,
        "tags": tags,
        "path": path,
        "size": len(text),
        "sha256": hashlib.sha256(text).hexdigest(),
        "fileName": path.split("/")[-1],
        "downloadUrl": RAW_URL + path,
    }


def expected(paths=None):
    """Path -> expected entry (None for no entry) for the staged Scripts, or for the given paths."""
    blobs = staged_blobs(paths)
    contents = read_blobs(list(set(blobs.values())))
    result = {p: None for p in paths or []}
    result.update({p: entry(p, contents[b]) for p, b in blobs.items()})
    return result


def update(entries, wanted):
    """Rewrites the entries in place; a new entry goes at the end, like a new file would."""
    index = {e["path"]: i for i, e in enumerate(entries)}
    for path, new in wanted.items():
        if path in index:
            entries[index[path]] = new
        elif new is not None:
            entries.append(new)
    return [e for e in entries if e is not None]


def dump(entries):
    # Byte for byte what Newtonsoft's indented output and Console.WriteLine give.
    return (json.dumps(entries, indent=2, ensure_ascii=False) + "\n").encode()


def sync(paths):
    if paths == ["--all"]:
        staged = expected()
        known = json.loads(git("show", ":" + SCRIPTS_JSON))
        wanted = {e["path"]: None for e in known} | staged
    else:
        wanted = expected(paths)

    staged_json = json.loads(git("show", ":" + SCRIPTS_JSON))
    blob = git("hash-object", "-w", "--stdin", stdin=dump(update(staged_json, wanted))).decode().strip()
    git("update-index", "--cacheinfo", f"100644,{blob},{SCRIPTS_JSON}")

    # The working tree gets the same entries, keeping any unstaged edits to other entries.
    with open(SCRIPTS_JSON, "rb") as f:
        working = json.loads(f.read())
    with open(SCRIPTS_JSON, "wb") as f:
        f.write(dump(update(working, wanted)))


def check():
    known = {e["path"]: e for e in json.loads(git("show", ":" + SCRIPTS_JSON))}
    wanted = expected()
    problems = []
    for path, new in sorted(wanted.items()):
        old = known.pop(path, None)
        if new is None:
            if old is not None:
                problems.append(f"{path}: has an entry but no /* header")
        elif old is None:
            problems.append(f"{path}: missing")
        elif old != new:
            fields = ", ".join(k for k in new if old.get(k) != new[k])
            problems.append(f"{path}: stale {fields}")
    problems += [f"{path}: entry for a file that doesn't exist" for path in sorted(known)]

    if problems:
        print(f"{SCRIPTS_JSON} is out of date:", *problems, sep="\n  ", file=sys.stderr)
        print("\nFix it with: python3 Tools/ForDevelopers/scripts_json.py sync --all", file=sys.stderr)
        return 1
    print(f"{SCRIPTS_JSON} matches all {sum(e is not None for e in wanted.values())} Scripts.")
    return 0


if __name__ == "__main__":
    command, args = (sys.argv[1], sys.argv[2:]) if len(sys.argv) > 1 else ("", [])
    top = git("rev-parse", "--show-toplevel").decode().strip()
    args = [a if a == "--all" else os.path.relpath(a, top).replace(os.sep, "/") for a in args]
    os.chdir(top)
    if command == "sync" and args:
        sync(args)
    elif command == "check" and not args:
        sys.exit(check())
    else:
        sys.exit(__doc__)
