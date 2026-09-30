"""
AQ Game Fetcher -- Neon edition.

Fetches AQW game resources:
  - Client        (Game####.swf)   -- via gameversion API, falls back to
                                       exponential+binary search if the API
                                       is ever unavailable
  - Game Assets   (Assets_YYYYMMDD.swf, date-stamped -- you supply the date)
  - Game Version  (JSON metadata)
  - Servers       (JSON server list)
  - ALL           -- runs every resource in sequence

Background worker, shared neon log, per-resource + Fetch All controls.
"""

import json
import os
import platform
import queue
import random
import subprocess
import threading
import time
import tkinter as tk
from datetime import datetime
from tkinter import ttk, scrolledtext

import requests

# --------------------------------------------------------------------------
# Config
# --------------------------------------------------------------------------

GAMEFILES_BASE = "https://game.aq.com/game/gamefiles/{}"
CLIENT_URL_TMPL = "https://game.aq.com/game/gamefiles/Game{}.swf"
ASSETS_URL_TMPL = "https://game.aq.com/game/gamefiles/interface/Assets/Assets_{}.swf"
GAMEVERSION_URL = "https://game.aq.com/game/api/data/gameversion"
SERVERS_URL = "https://game.aq.com/game/api/data/servers"

START_NUMBER = 3097
TIMEOUT = 12

MIN_DELAY = 0.12
MAX_DELAY = 0.40

MAX_RETRIES = 4
BACKOFF_BASE = 3

VERBOSE = False

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
STATE_FILE = os.path.join(SCRIPT_DIR, ".last_version")

HEADERS = {
    "User-Agent": (
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 "
        "(KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36"
    ),
    "Accept": "*/*",
    "Accept-Language": "en-US,en;q=0.9",
    "Connection": "keep-alive",
}

session = requests.Session()
session.headers.update(HEADERS)

# --------------------------------------------------------------------------
# Neon palette
# --------------------------------------------------------------------------

BG = "#0d0f14"
PANEL = "#151821"
CARD = "#1a1e2a"
CARD_HOVER = "#222838"
FG = "#e8eaef"
ACCENT = "#7c9cff"
ACCENT_DIM = "#5a78d4"
CYAN = "#56d4c8"
GREEN = "#7ee787"
YELLOW = "#e3b341"
RED = "#f7788a"
DIM = "#6b7385"
BORDER = "#2a3040"
GLOW = "#3d5afe"

FONT_UI = ("Segoe UI", 10)
FONT_UI_BOLD = ("Segoe UI Semibold", 10)
FONT_MONO = ("Consolas", 9)
FONT_TITLE = ("Segoe UI Semibold", 16)
FONT_SUB = ("Segoe UI", 9)
FONT_TINY = ("Segoe UI", 8)


class Cancelled(Exception):
    pass


# --------------------------------------------------------------------------
# Worker
# --------------------------------------------------------------------------

class Fetcher:
    """Runs resource fetch(es) on a background thread."""

    def __init__(self, out_queue: queue.Queue):
        self.q = out_queue
        self.cancel_flag = threading.Event()
        self.checks_done = 0

    def emit(self, kind, **payload):
        self.q.put((kind, payload))

    def log(self, msg, level="info"):
        self.emit("log", msg=msg, level=level)

    def status(self, msg):
        self.emit("status", msg=msg)

    def phase(self, title):
        self.emit("phase", title=title)

    def progress(self, pct):
        self.emit("progress", pct=pct)

    def get_json(self, url):
        resp = session.get(url, timeout=TIMEOUT)
        resp.raise_for_status()
        return resp.json()

    def download_file(self, url, dest_path, show_progress=True):
        start_time = time.time()
        with session.get(url, timeout=TIMEOUT, stream=True) as resp:
            resp.raise_for_status()
            total = int(resp.headers.get("Content-Length", 0))
            downloaded = 0
            with open(dest_path, "wb") as f:
                for chunk in resp.iter_content(chunk_size=131072):
                    if self.cancel_flag.is_set():
                        raise Cancelled()
                    if not chunk:
                        continue
                    f.write(chunk)
                    downloaded += len(chunk)
                    if total and show_progress:
                        pct = downloaded / total * 100
                        elapsed = max(time.time() - start_time, 1e-6)
                        speed = downloaded / elapsed
                        self.progress(pct)
                        self.status(
                            f"{downloaded/1024/1024:.1f}/{total/1024/1024:.1f} MB  "
                            f"·  {speed/1024/1024:.1f} MB/s"
                        )
        return time.time() - start_time

    def _looks_like_real_swf(self, resp):
        if resp.history and not resp.url.lower().endswith(".swf"):
            return False
        if "text/html" in resp.headers.get("Content-Type", "").lower():
            return False
        return True

    def _request_once(self, url):
        resp = session.head(url, timeout=TIMEOUT, allow_redirects=True)
        if resp.status_code in (405, 501):
            resp = session.get(url, timeout=TIMEOUT, stream=True)
            resp.close()
        return resp

    def exists(self, url):
        delay = BACKOFF_BASE
        for attempt in range(1, MAX_RETRIES + 1):
            if self.cancel_flag.is_set():
                raise Cancelled()
            try:
                resp = self._request_once(url)
            except requests.RequestException as e:
                if VERBOSE:
                    self.log(f"request failed ({attempt}/{MAX_RETRIES}): {e}", "warn")
            else:
                if resp.status_code == 429 or resp.status_code >= 500:
                    wait = float(resp.headers.get("Retry-After", delay))
                    time.sleep(wait)
                    delay *= 2
                    continue
                if resp.status_code != 200:
                    return False
                return self._looks_like_real_swf(resp)
            time.sleep(delay)
            delay *= 2
        return False

    def polite_pause(self):
        time.sleep(random.uniform(MIN_DELAY, MAX_DELAY))

    def load_last_version(self, default):
        try:
            with open(STATE_FILE, "r") as f:
                return int(f.read().strip())
        except (OSError, ValueError):
            return default

    def save_last_version(self, n):
        try:
            with open(STATE_FILE, "w") as f:
                f.write(str(n))
        except OSError as e:
            self.log(f"could not save last version: {e}", "warn")

    def _check(self, n):
        self.checks_done += 1
        found = self.exists(CLIENT_URL_TMPL.format(n))
        self.status(f"Probe Game{n}.swf  ·  {self.checks_done} checks")
        if found:
            self.log(f"Game{n}.swf found", "ok")
        self.polite_pause()
        return found

    def search_client_number(self):
        self.checks_done = 0
        start = self.load_last_version(START_NUMBER)
        self.phase("Fallback search · latest Game####.swf")

        if not self._check(start):
            if start != START_NUMBER:
                start = START_NUMBER
                if not self._check(start):
                    return None
            else:
                return None

        last_good = start
        step = 1
        while True:
            n = last_good + step
            if self._check(n):
                last_good = n
                step *= 2
            else:
                first_bad = n
                break

        lo, hi = last_good, first_bad
        while hi - lo > 1:
            mid = (lo + hi) // 2
            if self._check(mid):
                lo = mid
            else:
                hi = mid
        return lo

    # -- individual jobs -------------------------------------------------

    def run_client(self):
        self.phase("Client")
        try:
            self.status("Reading gameversion API…")
            data = self.get_json(GAMEVERSION_URL)
            sfile = data["sFile"]
            self.log(
                f"API → {sfile}  (build {data.get('sVersion', '?')})",
                "ok",
            )
        except Exception as e:
            self.log(f"API failed ({e}) · falling back to search", "warn")
            n = self.search_client_number()
            if n is None:
                self.log("Could not determine latest client version.", "error")
                self.emit("done", ok=False)
                return
            sfile = f"Game{n}.swf"

        url = GAMEFILES_BASE.format(sfile)
        dest = os.path.join(SCRIPT_DIR, sfile)
        self.status(f"Downloading {sfile}…")
        duration = self.download_file(url, dest)
        self.progress(100)
        self.log(f"Saved → {dest}", "ok")

        digits = "".join(c for c in sfile if c.isdigit())
        if digits:
            self.save_last_version(int(digits))

        self.emit("done", ok=True, dest=dest, summary=f"{sfile} · {duration:.1f}s")

    def run_assets(self, date_str):
        self.phase("Game Assets")
        fname = f"Assets_{date_str}.swf"
        url = ASSETS_URL_TMPL.format(date_str)
        dest = os.path.join(SCRIPT_DIR, fname)
        self.status(f"Downloading {fname}…")
        try:
            duration = self.download_file(url, dest)
        except requests.RequestException as e:
            self.log(f"Download failed: {e}", "error")
            self.log(
                "Assets are date-stamped and not in the version API — "
                "double-check the YYYYMMDD value.",
                "warn",
            )
            self.emit("done", ok=False)
            return
        self.progress(100)
        self.log(f"Saved → {dest}", "ok")
        self.emit("done", ok=True, dest=dest, summary=f"{fname} · {duration:.1f}s")

    def run_gameversion(self):
        self.phase("Game Version")
        try:
            self.status("Requesting gameversion API…")
            data = self.get_json(GAMEVERSION_URL)
        except Exception as e:
            self.log(f"Request failed: {e}", "error")
            self.emit("done", ok=False)
            return

        for k, v in data.items():
            self.log(f"{k}: {v}", "info")

        dest = os.path.join(SCRIPT_DIR, "gameversion.json")
        with open(dest, "w") as f:
            json.dump(data, f, indent=2)
        self.log(f"Saved → {dest}", "ok")
        self.emit("done", ok=True, dest=dest, summary="gameversion.json")

    def run_servers(self):
        self.phase("Servers")
        try:
            self.status("Requesting servers API…")
            data = self.get_json(SERVERS_URL)
        except Exception as e:
            self.log(f"Request failed: {e}", "error")
            self.emit("done", ok=False)
            return

        online = sum(1 for s in data if s.get("bOnline"))
        self.log(f"{len(data)} servers · {online} online", "ok")
        for s in sorted(data, key=lambda s: -s.get("iCount", 0)):
            self.log(
                f"  {s['sName']:<18} {s['iCount']:>4}/{s['iMax']:<4}  "
                f"{s['sIP']}:{s['iPort']}",
                "info",
            )

        dest = os.path.join(SCRIPT_DIR, "servers.json")
        with open(dest, "w") as f:
            json.dump(data, f, indent=2)
        self.log(f"Saved → {dest}", "ok")
        self.emit("done", ok=True, dest=dest, summary="servers.json")

    def run_all(self, date_str):
        """Sequential full pull: version → client → servers → assets."""
        jobs = [
            ("run_gameversion", ()),
            ("run_client", ()),
            ("run_servers", ()),
            ("run_assets", (date_str,)),
        ]
        total = len(jobs)
        results = []
        last_dest = SCRIPT_DIR

        for i, (name, args) in enumerate(jobs):
            if self.cancel_flag.is_set():
                self.log("All-fetch cancelled.", "warn")
                self.emit("done", ok=False)
                return

            self.emit("all_step", index=i + 1, total=total, name=name)
            # Reset per-job progress so the bar restarts cleanly
            self.progress(0)

            # Capture done via a temporary flag pattern: run method emits done,
            # but we intercept by re-binding emit temporarily is messy.
            # Instead, call the methods and let them emit; the GUI will
            # treat intermediate "done" as step-complete when in all-mode.
            method = getattr(self, name)
            # We need the method NOT to emit final done until the end.
            # Patch: run the body, then decide.
            try:
                if name == "run_gameversion":
                    self._all_gameversion()
                elif name == "run_client":
                    dest = self._all_client()
                    if dest:
                        last_dest = os.path.dirname(dest)
                elif name == "run_servers":
                    self._all_servers()
                elif name == "run_assets":
                    dest = self._all_assets(date_str)
                    if dest:
                        last_dest = os.path.dirname(dest)
                results.append(True)
            except Cancelled:
                self.log("All-fetch cancelled.", "warn")
                self.emit("done", ok=False)
                return
            except Exception as e:
                self.log(f"Step failed: {e}", "error")
                results.append(False)

            # small inter-job pause
            time.sleep(0.15)

        ok = all(results)
        summary = f"{sum(results)}/{total} resources"
        self.emit("done", ok=ok, dest=last_dest, summary=summary)

    # Internal all-mode variants that do NOT emit "done"
    def _all_gameversion(self):
        self.phase("Game Version")
        self.status("Requesting gameversion API…")
        data = self.get_json(GAMEVERSION_URL)
        for k, v in data.items():
            self.log(f"{k}: {v}", "info")
        dest = os.path.join(SCRIPT_DIR, "gameversion.json")
        with open(dest, "w") as f:
            json.dump(data, f, indent=2)
        self.log(f"Saved → {dest}", "ok")
        self.progress(100)

    def _all_client(self):
        self.phase("Client")
        try:
            self.status("Reading gameversion API…")
            data = self.get_json(GAMEVERSION_URL)
            sfile = data["sFile"]
            self.log(
                f"API → {sfile}  (build {data.get('sVersion', '?')})",
                "ok",
            )
        except Exception as e:
            self.log(f"API failed ({e}) · falling back to search", "warn")
            n = self.search_client_number()
            if n is None:
                self.log("Could not determine latest client version.", "error")
                raise RuntimeError("client version unknown")
            sfile = f"Game{n}.swf"

        url = GAMEFILES_BASE.format(sfile)
        dest = os.path.join(SCRIPT_DIR, sfile)
        self.status(f"Downloading {sfile}…")
        duration = self.download_file(url, dest)
        self.progress(100)
        self.log(f"Saved → {dest}  ({duration:.1f}s)", "ok")
        digits = "".join(c for c in sfile if c.isdigit())
        if digits:
            self.save_last_version(int(digits))
        return dest

    def _all_servers(self):
        self.phase("Servers")
        self.status("Requesting servers API…")
        data = self.get_json(SERVERS_URL)
        online = sum(1 for s in data if s.get("bOnline"))
        self.log(f"{len(data)} servers · {online} online", "ok")
        for s in sorted(data, key=lambda s: -s.get("iCount", 0)):
            self.log(
                f"  {s['sName']:<18} {s['iCount']:>4}/{s['iMax']:<4}  "
                f"{s['sIP']}:{s['iPort']}",
                "info",
            )
        dest = os.path.join(SCRIPT_DIR, "servers.json")
        with open(dest, "w") as f:
            json.dump(data, f, indent=2)
        self.log(f"Saved → {dest}", "ok")
        self.progress(100)

    def _all_assets(self, date_str):
        self.phase("Game Assets")
        fname = f"Assets_{date_str}.swf"
        url = ASSETS_URL_TMPL.format(date_str)
        dest = os.path.join(SCRIPT_DIR, fname)
        self.status(f"Downloading {fname}…")
        duration = self.download_file(url, dest)
        self.progress(100)
        self.log(f"Saved → {dest}  ({duration:.1f}s)", "ok")
        return dest


# --------------------------------------------------------------------------
# Resource card
# --------------------------------------------------------------------------

class ResourceCard(tk.Frame):
    """Compact card: icon · title · subtitle · optional input · status · Fetch."""

    def __init__(self, master, icon, title, subtitle, on_fetch, extra_widget=None):
        super().__init__(master, bg=CARD, highlightthickness=1,
                         highlightbackground=BORDER, highlightcolor=ACCENT)
        self.on_fetch = on_fetch

        inner = tk.Frame(self, bg=CARD)
        inner.pack(fill="x", padx=14, pady=12)

        # left: icon + text
        left = tk.Frame(inner, bg=CARD)
        left.pack(side="left", fill="x", expand=True)

        top_row = tk.Frame(left, bg=CARD)
        top_row.pack(fill="x")
        tk.Label(top_row, text=icon, font=("Segoe UI", 14),
                 bg=CARD, fg=ACCENT).pack(side="left", padx=(0, 8))
        tk.Label(top_row, text=title, font=FONT_UI_BOLD,
                 bg=CARD, fg=FG, anchor="w").pack(side="left")

        tk.Label(left, text=subtitle, font=FONT_TINY,
                 bg=CARD, fg=DIM, anchor="w").pack(fill="x", pady=(2, 0))

        if extra_widget is not None:
            extra_widget(left).pack(anchor="w", pady=(6, 0))

        # right: status + button
        self.status_var = tk.StringVar(value="")
        tk.Label(inner, textvariable=self.status_var, font=FONT_TINY,
                 bg=CARD, fg=CYAN, width=14, anchor="e").pack(
            side="left", padx=(8, 10))

        self.btn = tk.Button(
            inner, text="Fetch", font=FONT_UI_BOLD, command=self._fetch,
            bg=ACCENT, fg="#0a0c12", activebackground=ACCENT_DIM,
            activeforeground="#0a0c12", relief="flat", padx=16, pady=5,
            cursor="hand2", borderwidth=0,
        )
        self.btn.pack(side="right")

    def _fetch(self):
        self.on_fetch(self)

    def set_busy(self, busy):
        self.btn.configure(state="disabled" if busy else "normal")

    def set_status(self, text):
        self.status_var.set(text)


# --------------------------------------------------------------------------
# App
# --------------------------------------------------------------------------

class App(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("AQ Game Fetcher  ·  Neon")
        self.geometry("680x720")
        self.minsize(560, 560)
        self.configure(bg=BG)

        self.worker = None
        self.fetcher = None
        self.msg_queue = queue.Queue()
        self.active_row = None
        self.last_dest_dir = SCRIPT_DIR
        self.all_mode = False

        self.assets_date_var = tk.StringVar(value="20250328")

        self._build_ui()
        self.after(60, self._poll_queue)

    def _build_ui(self):
        # ── header ────────────────────────────────────────────────────
        header = tk.Frame(self, bg=BG)
        header.pack(fill="x", padx=20, pady=(18, 4))

        title_row = tk.Frame(header, bg=BG)
        title_row.pack(fill="x")
        tk.Label(title_row, text="◈", font=("Segoe UI", 18),
                 bg=BG, fg=ACCENT).pack(side="left", padx=(0, 8))
        tk.Label(title_row, text="AQ Game Fetcher", font=FONT_TITLE,
                 bg=BG, fg=FG).pack(side="left")
        tk.Label(title_row, text="neon", font=("Segoe UI", 9),
                 bg=BG, fg=CYAN).pack(side="left", padx=(10, 0), pady=(6, 0))

        tk.Label(header, text="game.aq.com  ·  client · assets · version · servers",
                 font=FONT_SUB, bg=BG, fg=DIM).pack(anchor="w", pady=(2, 0))

        # status line
        self.status_var = tk.StringVar(value="Ready.")
        tk.Label(self, textvariable=self.status_var, font=FONT_UI,
                 bg=BG, fg=CYAN, anchor="w").pack(fill="x", padx=20, pady=(8, 0))

        # progress
        style = ttk.Style(self)
        style.theme_use("default")
        style.configure(
            "Neon.Horizontal.TProgressbar",
            troughcolor=PANEL,
            background=ACCENT,
            bordercolor=PANEL,
            lightcolor=ACCENT,
            darkcolor=ACCENT,
            thickness=8,
        )
        self.progress_var = tk.DoubleVar(value=0)
        ttk.Progressbar(
            self, variable=self.progress_var, maximum=100,
            style="Neon.Horizontal.TProgressbar",
        ).pack(fill="x", padx=20, pady=(6, 14))

        # ── Fetch All banner ──────────────────────────────────────────
        all_frame = tk.Frame(self, bg=PANEL, highlightthickness=1,
                             highlightbackground=GLOW)
        all_frame.pack(fill="x", padx=20, pady=(0, 10))

        all_inner = tk.Frame(all_frame, bg=PANEL)
        all_inner.pack(fill="x", padx=14, pady=12)

        left_all = tk.Frame(all_inner, bg=PANEL)
        left_all.pack(side="left", fill="x", expand=True)
        tk.Label(left_all, text="⚡  Fetch All", font=FONT_UI_BOLD,
                 bg=PANEL, fg=FG).pack(anchor="w")
        tk.Label(left_all, text="Version → Client → Servers → Assets  ·  sequential",
                 font=FONT_TINY, bg=PANEL, fg=DIM).pack(anchor="w")

        self.all_btn = tk.Button(
            all_inner, text="Fetch All", font=FONT_UI_BOLD,
            command=self.fetch_all,
            bg=GLOW, fg="#e8eaef", activebackground="#536dfe",
            activeforeground="#e8eaef", relief="flat", padx=20, pady=8,
            cursor="hand2", borderwidth=0,
        )
        self.all_btn.pack(side="right")

        # ── resource cards ────────────────────────────────────────────
        cards = tk.Frame(self, bg=BG)
        cards.pack(fill="x", padx=20)

        def make_card(icon, title, subtitle, handler, extra=None):
            card = ResourceCard(cards, icon, title, subtitle, handler, extra)
            card.pack(fill="x", pady=4)
            return card

        self.client_row = make_card(
            "◆", "Client",
            "Game####.swf  ·  gameversion API  ·  search fallback",
            self.fetch_client,
        )

        def assets_extra(parent):
            f = tk.Frame(parent, bg=CARD)
            tk.Label(f, text="Date  YYYYMMDD", font=FONT_TINY,
                     bg=CARD, fg=DIM).pack(side="left")
            entry = tk.Entry(
                f, textvariable=self.assets_date_var, width=11,
                bg=PANEL, fg=FG, insertbackground=FG,
                relief="flat", font=FONT_MONO,
                highlightthickness=1, highlightbackground=BORDER,
                highlightcolor=ACCENT,
            )
            entry.pack(side="left", padx=(8, 0), ipady=3)
            return f

        self.assets_row = make_card(
            "◇", "Game Assets",
            "Assets_YYYYMMDD.swf  ·  date-stamped, set manually",
            self.fetch_assets, extra=assets_extra,
        )

        self.version_row = make_card(
            "▣", "Game Version",
            "JSON metadata  ·  current file, title, build",
            self.fetch_version,
        )

        self.servers_row = make_card(
            "☰", "Servers",
            "JSON server list  ·  live population counts",
            self.fetch_servers,
        )

        self.rows = [
            self.client_row, self.assets_row,
            self.version_row, self.servers_row,
        ]

        # ── log ───────────────────────────────────────────────────────
        log_wrap = tk.Frame(self, bg=PANEL, highlightthickness=1,
                            highlightbackground=BORDER)
        log_wrap.pack(fill="both", expand=True, padx=20, pady=(12, 0))

        log_hdr = tk.Frame(log_wrap, bg=PANEL)
        log_hdr.pack(fill="x", padx=10, pady=(8, 0))
        tk.Label(log_hdr, text="LOG", font=FONT_TINY,
                 bg=PANEL, fg=DIM).pack(side="left")

        self.log_box = scrolledtext.ScrolledText(
            log_wrap, bg=PANEL, fg=FG, insertbackground=FG,
            font=FONT_MONO, wrap="word", borderwidth=0,
            highlightthickness=0, state="disabled",
            padx=8, pady=6,
        )
        self.log_box.pack(fill="both", expand=True, padx=4, pady=4)

        for tag, color in (
            ("ok", GREEN), ("warn", YELLOW), ("error", RED),
            ("info", DIM), ("phase", ACCENT), ("time", DIM),
        ):
            self.log_box.tag_configure(tag, foreground=color)

        # ── bottom bar ────────────────────────────────────────────────
        bar = tk.Frame(self, bg=BG)
        bar.pack(fill="x", padx=20, pady=16)

        self.cancel_btn = tk.Button(
            bar, text="Cancel", font=FONT_UI, command=self.cancel_fetch,
            bg=RED, fg="#0a0c12", activebackground="#d45a6c",
            relief="flat", padx=16, pady=7, cursor="hand2",
            state="disabled", borderwidth=0,
        )
        self.cancel_btn.pack(side="left")

        self.open_btn = tk.Button(
            bar, text="Open Folder", font=FONT_UI, command=self.open_folder,
            bg=CARD, fg=FG, activebackground=CARD_HOVER,
            relief="flat", padx=14, pady=7, cursor="hand2", borderwidth=0,
        )
        self.open_btn.pack(side="left", padx=(8, 0))

        self.clear_btn = tk.Button(
            bar, text="Clear Log", font=FONT_UI, command=self.clear_log,
            bg=CARD, fg=FG, activebackground=CARD_HOVER,
            relief="flat", padx=14, pady=7, cursor="hand2", borderwidth=0,
        )
        self.clear_btn.pack(side="left", padx=(8, 0))

        tk.Label(bar, text="files land next to this script",
                 font=FONT_TINY, bg=BG, fg=DIM).pack(side="right")

    # -- log -------------------------------------------------------------

    def append_log(self, msg, tag="info"):
        self.log_box.configure(state="normal")
        ts = datetime.now().strftime("%H:%M:%S")
        prefix = {
            "ok": "  ✓  ", "warn": "  !  ", "error": "  ✗  ",
            "phase": "▶ ", "info": "     ",
        }.get(tag, "     ")
        if tag == "phase":
            self.log_box.insert("end", f"\n{ts}  ", "time")
            self.log_box.insert("end", f"{prefix}{msg}\n", tag)
        else:
            self.log_box.insert("end", f"{ts}  ", "time")
            self.log_box.insert("end", f"{prefix}{msg}\n", tag)
        self.log_box.see("end")
        self.log_box.configure(state="disabled")

    # -- job control -----------------------------------------------------

    def _set_busy(self, busy):
        for r in self.rows:
            r.set_busy(busy)
        self.all_btn.configure(state="disabled" if busy else "normal")
        self.cancel_btn.configure(state="normal" if busy else "disabled")

    def _start_job(self, row, target_name, *args, all_mode=False):
        if self.worker and self.worker.is_alive():
            return
        self.active_row = row
        self.all_mode = all_mode
        self.progress_var.set(0)
        self.status_var.set("Starting…")
        if row:
            row.set_status("Working…")
        self._set_busy(True)

        self.msg_queue = queue.Queue()
        self.fetcher = Fetcher(self.msg_queue)
        target = getattr(self.fetcher, target_name)
        self.worker = threading.Thread(
            target=target, args=args, daemon=True
        )
        self.worker.start()

    def fetch_client(self, row):
        self._start_job(row, "run_client")

    def fetch_assets(self, row):
        date_str = self.assets_date_var.get().strip()
        if not (len(date_str) == 8 and date_str.isdigit()):
            self.append_log("Assets date must be 8 digits, e.g. 20250328", "error")
            return
        self._start_job(row, "run_assets", date_str)

    def fetch_version(self, row):
        self._start_job(row, "run_gameversion")

    def fetch_servers(self, row):
        self._start_job(row, "run_servers")

    def fetch_all(self):
        date_str = self.assets_date_var.get().strip()
        if not (len(date_str) == 8 and date_str.isdigit()):
            self.append_log(
                "Assets date must be 8 digits before Fetch All, e.g. 20250328",
                "error",
            )
            return
        self._start_job(None, "run_all", date_str, all_mode=True)

    def cancel_fetch(self):
        if self.fetcher:
            self.fetcher.cancel_flag.set()
        self.status_var.set("Cancelling…")

    def open_folder(self):
        path = self.last_dest_dir
        system = platform.system()
        try:
            if system == "Windows":
                os.startfile(path)  # type: ignore[attr-defined]
            elif system == "Darwin":
                subprocess.run(["open", path], check=False)
            else:
                subprocess.run(["xdg-open", path], check=False)
        except Exception as e:
            self.append_log(f"could not open folder: {e}", "warn")

    def clear_log(self):
        self.log_box.configure(state="normal")
        self.log_box.delete("1.0", "end")
        self.log_box.configure(state="disabled")

    def _job_finished(self):
        self._set_busy(False)
        if self.active_row:
            self.active_row.set_status("")
        self.active_row = None
        self.all_mode = False

    # -- queue poll ------------------------------------------------------

    def _poll_queue(self):
        try:
            while True:
                kind, payload = self.msg_queue.get_nowait()
                if kind == "log":
                    self.append_log(payload["msg"], payload.get("level", "info"))
                elif kind == "status":
                    self.status_var.set(payload["msg"])
                    if self.active_row:
                        self.active_row.set_status(payload["msg"][:18])
                elif kind == "phase":
                    self.append_log(payload["title"], "phase")
                elif kind == "progress":
                    self.progress_var.set(payload["pct"])
                elif kind == "all_step":
                    i, t = payload["index"], payload["total"]
                    self.status_var.set(f"All  ·  step {i}/{t}")
                elif kind == "done":
                    if payload.get("ok"):
                        self.status_var.set(
                            f"Done  ·  {payload.get('summary', '')}"
                        )
                        dest = payload.get("dest")
                        if dest:
                            self.last_dest_dir = (
                                dest if os.path.isdir(dest)
                                else os.path.dirname(dest)
                            )
                    else:
                        self.status_var.set("Failed  ·  see log")
                    self._job_finished()
        except queue.Empty:
            pass
        self.after(60, self._poll_queue)


if __name__ == "__main__":
    App().mainloop()