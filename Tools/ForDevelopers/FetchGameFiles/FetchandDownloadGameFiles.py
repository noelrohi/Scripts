"""
AQ Game Fetcher -- Futuristic Cyber-HUD Edition (Rate-Limit Protected).

Fetches AQW game resources:
  - Client        (Game####.swf)   -- via gameversion API with 429 backoff protection
  - Game Assets   (Assets_YYYYMMDD.swf -- instant resolution)
  - Game Version  (JSON metadata)
  - Servers       (JSON server list)
  - ALL           -- runs every resource in sequence with rate-limit pacing
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
from datetime import datetime, timedelta
from tkinter import ttk, scrolledtext

import requests

# --------------------------------------------------------------------------
# Config & Endpoints
# --------------------------------------------------------------------------

GAMEFILES_BASE = "https://game.aq.com/game/gamefiles/{}"
CLIENT_URL_TMPL = "https://game.aq.com/game/gamefiles/Game{}.swf"
ASSETS_URL_TMPL = "https://game.aq.com/game/gamefiles/interface/Assets/Assets_{}.swf"
GAMEVERSION_URL = "https://game.aq.com/game/api/data/gameversion"
SERVERS_URL = "https://game.aq.com/game/api/data/servers"

START_NUMBER = 3097
TIMEOUT = 12

MIN_DELAY = 0.15
MAX_DELAY = 0.30

MAX_RETRIES = 5
BACKOFF_BASE = 3

VERBOSE = False

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
STATE_FILE = os.path.join(SCRIPT_DIR, ".last_version")
ASSET_STATE_FILE = os.path.join(SCRIPT_DIR, ".last_assets_date")

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
# Ultimate Cyber-HUD Palette & Typography
# --------------------------------------------------------------------------

BG = "#06080c"          # Ultra-deep obsidian void
PANEL = "#0a0e17"       # Sleek dark cyber panel
CARD = "#0f1522"        # Layered card surface
CARD_HOVER = "#161f33"  # Luminous glowing hover state
FG = "#f8fafc"          # Crisp high-contrast text
ACCENT = "#38bdf8"      # Electric Sky Cyan
ACCENT_DIM = "#0284c7"  # Deep cyan action
CYAN = "#2dd4bf"        # Neon Teal
GREEN = "#4ade80"       # Cyber Green
YELLOW = "#facc15"      # Warning Amber
RED = "#fb7185"         # Alert Crimson
DIM = "#64748b"         # Muted telemetry slate
BORDER = "#1e293b"      # Crisp structural grid lines
GLOW = "#6366f1"        # Indigo-Violet Beam
GLOW_HOVER = "#818cf8"  # Bright Indigo Beam

FONT_UI = ("Segoe UI", 10)
FONT_UI_BOLD = ("Segoe UI Semibold", 10)
FONT_MONO = ("Consolas", 9)
FONT_TITLE = ("Segoe UI Semibold", 15)
FONT_SUB = ("Segoe UI", 9)
FONT_TINY = ("Segoe UI", 8)


class Cancelled(Exception):
    pass


# --------------------------------------------------------------------------
# UI Helpers for Rounded Shapes & HUD Elements
# --------------------------------------------------------------------------

def draw_rounded_rect(canvas, x1, y1, x2, y2, radius=12, **kwargs):
    """Draws a smooth rounded rectangle on a canvas with high fidelity."""
    points = [
        x1 + radius, y1,
        x1 + radius, y1,
        x2 - radius, y1,
        x2 - radius, y1,
        x2, y1,
        x2, y1 + radius,
        x2, y1 + radius,
        x2, y2 - radius,
        x2, y2 - radius,
        x2, y2,
        x2 - radius, y2,
        x2 - radius, y2,
        x1 + radius, y2,
        x1 + radius, y2,
        x1, y2,
        x1, y2 - radius,
        x1, y2 - radius,
        x1, y1 + radius,
        x1, y1 + radius,
        x1, y1
    ]
    return canvas.create_polygon(points, smooth=True, **kwargs)


class RoundedButton(tk.Canvas):
    """A high-end futuristic pill button with smooth luminous hover reactions."""
    def __init__(self, master, text, command, bg_color=ACCENT, fg_color="#06080c", 
                 hover_color=ACCENT_DIM, width=115, height=32, radius=8, font=FONT_UI_BOLD):
        super().__init__(master, width=width, height=height, bg=master["bg"], 
                         highlightthickness=0, cursor="hand2")
        self.command = command
        self.bg_color = bg_color
        self.hover_color = hover_color
        self.fg_color = fg_color
        self.radius = radius
        self.width = width
        self.height = height
        self.text = text
        self.font = font
        self.state_val = "normal"

        self.rect = draw_rounded_rect(self, 1, 1, width - 1, height - 1, radius=radius, fill=bg_color, outline="")
        self.text_id = self.create_text(width / 2, height / 2, text=text, fill=fg_color, font=font)

        self.bind("<Enter>", self._on_enter)
        self.bind("<Leave>", self._on_leave)
        self.bind("<Button-1>", self._on_click)
        self.tag_bind(self.rect, "<Button-1>", self._on_click)
        self.tag_bind(self.text_id, "<Button-1>", self._on_click)

    def _on_enter(self, event):
        if self.state_val == "normal":
            self.itemconfig(self.rect, fill=self.hover_color)

    def _on_leave(self, event):
        if self.state_val == "normal":
            self.itemconfig(self.rect, fill=self.bg_color)

    def _on_click(self, event):
        if self.state_val == "normal" and self.command:
            self.command()

    def configure_state(self, state):
        self.state_val = state
        if state == "disabled":
            self.itemconfig(self.rect, fill=BORDER)
            self.itemconfig(self.text_id, fill=DIM)
            self.configure(cursor="")
        else:
            self.itemconfig(self.rect, fill=self.bg_color)
            self.itemconfig(self.text_id, fill=self.fg_color)
            self.configure(cursor="hand2")


# --------------------------------------------------------------------------
# Worker Thread Logic with 429 Rate-Limit Safeguards
# --------------------------------------------------------------------------

class Fetcher:
    """Runs resource fetch(es) on a background thread safely with rate-limit protection."""

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

    def get_json_with_retry(self, url):
        """Fetches JSON with automatic retry handling for HTTP 429 Too Many Requests."""
        delay = BACKOFF_BASE
        for attempt in range(1, MAX_RETRIES + 1):
            if self.cancel_flag.is_set():
                raise Cancelled()
            try:
                resp = session.get(url, timeout=TIMEOUT)
                if resp.status_code == 429:
                    self.status(f"Rate limited (429). Retrying in {delay}s...")
                    time.sleep(delay)
                    delay *= 2
                    continue
                resp.raise_for_status()
                return resp.json()
            except requests.RequestException as e:
                if attempt == MAX_RETRIES:
                    raise e
                time.sleep(delay)
                delay *= 2
        raise requests.RequestException(f"Failed to fetch {url} after {MAX_RETRIES} attempts.")

    def download_file(self, url, dest_path, show_progress=True):
        start_time = time.time()
        delay = BACKOFF_BASE
        for attempt in range(1, MAX_RETRIES + 1):
            if self.cancel_flag.is_set():
                raise Cancelled()
            try:
                with session.get(url, timeout=TIMEOUT, stream=True) as resp:
                    if resp.status_code == 429:
                        time.sleep(delay)
                        delay *= 2
                        continue
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
            except requests.RequestException as e:
                if attempt == MAX_RETRIES:
                    raise e
                time.sleep(delay)
                delay *= 2
        return 0

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
            except requests.RequestException:
                pass
            else:
                if resp.status_code == 429:
                    time.sleep(delay)
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
                val = f.read().strip()
                if val.isdigit():
                    return int(val)
                digits_prefix = "".join(c for c in val if c.isdigit())
                return int(digits_prefix) if digits_prefix else default
        except (OSError, ValueError):
            return default

    def save_last_version(self, version_str):
        try:
            with open(STATE_FILE, "w") as f:
                f.write(str(version_str))
        except OSError as e:
            self.log(f"could not save last version: {e}", "warn")

    def load_last_assets_date(self):
        try:
            with open(ASSET_STATE_FILE, "r") as f:
                val = f.read().strip()
                if len(val) == 8 and val.isdigit():
                    return val
        except OSError:
            pass
        return None

    def save_last_assets_date(self, date_str):
        try:
            with open(ASSET_STATE_FILE, "w") as f:
                f.write(str(date_str))
        except OSError:
            pass

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

    def find_latest_assets_date(self, given_date_str=""):
        if given_date_str and len(given_date_str) == 8 and given_date_str.isdigit():
            if self.exists(ASSETS_URL_TMPL.format(given_date_str)):
                return given_date_str

        self.status("Resolving latest assets package…")
        last_saved = self.load_last_assets_date()
        if last_saved and self.exists(ASSETS_URL_TMPL.format(last_saved)):
            return last_saved

        curr = datetime.now()
        for i in range(10):
            if self.cancel_flag.is_set():
                raise Cancelled()
            d_str = (curr - timedelta(days=i)).strftime("%Y%m%d")
            url = ASSETS_URL_TMPL.format(d_str)
            if self.exists(url):
                return d_str
            self.polite_pause()

        return None

    # -- individual jobs -------------------------------------------------

    def run_client(self, emit_done=True):
        self.phase("Client")
        try:
            self.status("Reading gameversion API…")
            data = self.get_json_with_retry(GAMEVERSION_URL)
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
                if emit_done:
                    self.emit("done", ok=False)
                return None
            sfile = f"Game{n}.swf"

        url = GAMEFILES_BASE.format(sfile)
        dest = os.path.join(SCRIPT_DIR, sfile)
        self.status(f"Downloading {sfile}…")
        duration = self.download_file(url, dest)
        self.progress(100)
        self.log(f"Saved → {dest}", "ok")

        version_id = sfile.replace("Game", "").replace(".swf", "")
        if version_id:
            self.save_last_version(version_id)

        if emit_done:
            self.emit("done", ok=True, dest=dest, summary=f"{sfile} · {duration:.1f}s")
        return dest

    def run_assets(self, date_str="", emit_done=True):
        self.phase("Game Assets")
        
        resolved_date = self.find_latest_assets_date(date_str)
        if not resolved_date:
            self.log("Could not find any valid recent Assets file automatically.", "error")
            if emit_done:
                self.emit("done", ok=False)
            return None

        self.save_last_assets_date(resolved_date)
        if resolved_date != date_str:
            self.log(f"Auto-resolved active assets release → Assets_{resolved_date}.swf", "ok")

        fname = f"Assets_{resolved_date}.swf"
        url = ASSETS_URL_TMPL.format(resolved_date)
        dest = os.path.join(SCRIPT_DIR, fname)
        self.status(f"Downloading {fname}…")
        try:
            duration = self.download_file(url, dest)
        except requests.RequestException as e:
            self.log(f"Download failed: {e}", "error")
            if emit_done:
                self.emit("done", ok=False)
            return None
        self.progress(100)
        self.log(f"Saved → {dest}", "ok")
        if emit_done:
            self.emit("done", ok=True, dest=dest, summary=f"{fname} · {duration:.1f}s")
        return dest

    def run_gameversion(self, emit_done=True):
        self.phase("Game Version")
        try:
            self.status("Requesting gameversion API…")
            data = self.get_json_with_retry(GAMEVERSION_URL)
        except Exception as e:
            self.log(f"Request failed: {e}", "error")
            if emit_done:
                self.emit("done", ok=False)
            return None

        for k, v in data.items():
            self.log(f"{k}: {v}", "info")

        dest = os.path.join(SCRIPT_DIR, "gameversion.json")
        with open(dest, "w") as f:
            json.dump(data, f, indent=2)
        self.log(f"Saved → {dest}", "ok")
        self.progress(100)
        if emit_done:
            self.emit("done", ok=True, dest=dest, summary="gameversion.json")
        return dest

    def run_servers(self, emit_done=True):
        self.phase("Servers")
        try:
            self.status("Requesting servers API…")
            data = self.get_json_with_retry(SERVERS_URL)
        except Exception as e:
            self.log(f"Request failed: {e}", "error")
            if emit_done:
                self.emit("done", ok=False)
            return None

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
        if emit_done:
            self.emit("done", ok=True, dest=dest, summary="servers.json")
        return dest

    def run_all(self, date_str):
        """Sequential full pull with proper rate-limit pacing between endpoints."""
        jobs = [
            ("run_gameversion", (False,)),
            ("run_client", (False,)),
            ("run_servers", (False,)),
            ("run_assets", (date_str, False)),
        ]
        total = len(jobs)
        results = []
        last_dest = SCRIPT_DIR

        for i, (method_name, args) in enumerate(jobs):
            if self.cancel_flag.is_set():
                self.log("All-fetch cancelled.", "warn")
                self.emit("done", ok=False)
                return

            self.emit("all_step", index=i + 1, total=total, name=method_name)
            self.progress(0)

            method = getattr(self, method_name)
            try:
                dest = method(*args)
                if dest:
                    last_dest = os.path.dirname(dest) if not os.path.isdir(dest) else dest
                results.append(dest is not None)
            except Cancelled:
                self.log("All-fetch cancelled.", "warn")
                self.emit("done", ok=False)
                return
            except Exception as e:
                self.log(f"Step failed: {e}", "error")
                results.append(False)

            # Polite buffer between batch steps to prevent 429 rate limits
            time.sleep(0.4)

        ok = all(results)
        summary = f"{sum(results)}/{total} resources"
        self.emit("done", ok=ok, dest=last_dest, summary=summary)


# --------------------------------------------------------------------------
# Resource Card (Stylized Holographic HUD Card)
# --------------------------------------------------------------------------

class ResourceCard(tk.Canvas):
    """Stylized holographic card container with glowing telemetry border accents."""

    def __init__(self, master, icon, title, subtitle, on_fetch, extra_widget=None):
        super().__init__(master, bg=BG, height=74, highlightthickness=0)
        self.on_fetch = on_fetch
        self.bg_color = CARD

        self.bind("<Configure>", self._draw)
        
        self.inner = tk.Frame(self, bg=CARD)
        self.inner_id = self.create_window(10, 5, anchor="nw", window=self.inner)

        left = tk.Frame(self.inner, bg=CARD)
        left.pack(side="left", fill="x", expand=True, padx=10, pady=8)

        top_row = tk.Frame(left, bg=CARD)
        top_row.pack(fill="x")
        tk.Label(top_row, text=icon, font=("Segoe UI", 12), bg=CARD, fg=ACCENT).pack(side="left", padx=(0, 8))
        tk.Label(top_row, text=title, font=FONT_UI_BOLD, bg=CARD, fg=FG, anchor="w").pack(side="left")

        tk.Label(left, text=subtitle, font=FONT_TINY, bg=CARD, fg=DIM, anchor="w").pack(fill="x", pady=(2, 0))

        if extra_widget is not None:
            extra_widget(left).pack(anchor="w", pady=(4, 0))

        right = tk.Frame(self.inner, bg=CARD)
        right.pack(side="right", padx=8)

        self.status_var = tk.StringVar(value="")
        tk.Label(right, textvariable=self.status_var, font=FONT_TINY,
                 bg=CARD, fg=CYAN, width=16, anchor="e").pack(side="left", padx=(0, 10))

        self.btn = RoundedButton(right, text="FETCH", command=self._fetch, 
                                 bg_color=ACCENT, hover_color=ACCENT_DIM, width=82, height=30, radius=7, font=FONT_TINY)
        self.btn.pack(side="left")

        self.bind("<Enter>", self._on_enter)
        self.bind("<Leave>", self._on_leave)

    def _draw(self, event=None):
        self.delete("bg_rect")
        w = self.winfo_width()
        h = self.winfo_height()
        if w > 10 and h > 10:
            self.rect = draw_rounded_rect(self, 2, 2, w - 2, h - 2, radius=12, fill=self.bg_color, outline=BORDER, width=1)
            self.stripe = draw_rounded_rect(self, 2, 8, 5, h - 8, radius=2, fill=ACCENT, outline="")
            self.tag_lower(self.rect)
            self.coords(self.inner_id, 10, 5)
            self.itemconfig(self.inner_id, width=w - 20, height=h - 10)

    def _on_enter(self, event):
        self.bg_color = CARD_HOVER
        self._draw()

    def _on_leave(self, event):
        self.bg_color = CARD
        self._draw()

    def _fetch(self):
        self.on_fetch(self)

    def set_busy(self, busy):
        self.btn.configure_state("disabled" if busy else "normal")

    def set_status(self, text):
        self.status_var.set(text)


# --------------------------------------------------------------------------
# Main App Window
# --------------------------------------------------------------------------

class App(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("AQ Game Fetcher  ·  Cyber-HUD Studio")
        self.geometry("720x780")
        self.minsize(580, 580)
        self.configure(bg=BG)

        self.rowconfigure(4, weight=1)
        self.columnconfigure(0, weight=1)

        self.worker = None
        self.fetcher = None
        self.msg_queue = queue.Queue()
        self.active_row = None
        self.last_dest_dir = SCRIPT_DIR
        self.all_mode = False

        self.assets_date_var = tk.StringVar(value="")

        self._build_ui()
        self.after(60, self._poll_queue)

    def _build_ui(self):
        header = tk.Frame(self, bg=BG)
        header.grid(row=0, column=0, sticky="ew", padx=20, pady=(18, 4))

        title_row = tk.Frame(header, bg=BG)
        title_row.pack(fill="x")
        tk.Label(title_row, text="✦", font=("Segoe UI", 16), bg=BG, fg=ACCENT).pack(side="left", padx=(0, 8))
        tk.Label(title_row, text="AQ GAME FETCHER", font=FONT_TITLE, bg=BG, fg=FG).pack(side="left")
        
        badge = tk.Frame(title_row, bg=PANEL, highlightthickness=1, highlightbackground=ACCENT)
        badge.pack(side="left", padx=(14, 0), pady=2)
        tk.Label(badge, text=" ● SYS_ONLINE ", font=("Consolas", 8, "bold"), bg=PANEL, fg=GREEN).pack(padx=6, pady=2)

        tk.Label(header, text="GAME.AQ.COM  //  SECURE RESOURCE TELEMETRY & ACQUISITION SUITE",
                 font=("Consolas", 8), bg=BG, fg=DIM).pack(anchor="w", pady=(4, 0))

        self.status_var = tk.StringVar(value="STATUS: READY FOR TELEMETRY SYNC.")
        tk.Label(self, textvariable=self.status_var, font=("Consolas", 9),
                 bg=BG, fg=CYAN, anchor="w").grid(row=1, column=0, sticky="ew", padx=20, pady=(6, 0))

        style = ttk.Style(self)
        style.theme_use("default")
        style.configure(
            "Neon.Horizontal.TProgressbar",
            troughcolor=PANEL,
            background=ACCENT,
            bordercolor=PANEL,
            lightcolor=ACCENT,
            darkcolor=ACCENT,
            thickness=6,
        )
        self.progress_var = tk.DoubleVar(value=0)
        ttk.Progressbar(
            self, variable=self.progress_var, maximum=100,
            style="Neon.Horizontal.TProgressbar",
        ).grid(row=2, column=0, sticky="ew", padx=20, pady=(6, 12))

        top_container = tk.Frame(self, bg=BG)
        top_container.grid(row=3, column=0, sticky="ew", padx=20, pady=(0, 8))
        top_container.columnconfigure(0, weight=1)

        all_canvas = tk.Canvas(top_container, bg=BG, height=62, highlightthickness=0)
        all_canvas.pack(fill="x", pady=(0, 8))

        def draw_all_bg(event=None):
            all_canvas.delete("bg")
            w = all_canvas.winfo_width()
            h = all_canvas.winfo_height()
            if w > 10:
                draw_rounded_rect(all_canvas, 2, 2, w - 2, h - 2, radius=12, fill=PANEL, outline=GLOW, width=1, tag="bg")
                all_canvas.tag_lower("bg")
                all_canvas.coords(all_win, 16, 6)
                all_canvas.itemconfig(all_win, width=w - 32, height=h - 12)

        all_canvas.bind("<Configure>", draw_all_bg)

        all_inner = tk.Frame(all_canvas, bg=PANEL)
        all_win = all_canvas.create_window(16, 6, anchor="nw", window=all_inner)

        left_all = tk.Frame(all_inner, bg=PANEL)
        left_all.pack(side="left", fill="x", expand=True, pady=4)
        tk.Label(left_all, text="⚡  SEQUENCE FETCH ALL", font=FONT_UI_BOLD, bg=PANEL, fg=FG).pack(anchor="w")
        tk.Label(left_all, text="Version → Client → Servers → Assets  (Rate-Limit Protected)",
                 font=FONT_TINY, bg=PANEL, fg=DIM).pack(anchor="w")

        self.all_btn = RoundedButton(
            all_inner, text="EXECUTE ALL", command=self.fetch_all,
            bg_color=GLOW, hover_color=GLOW_HOVER, fg_color=FG,
            width=120, height=32, radius=8, font=FONT_UI_BOLD
        )
        self.all_btn.pack(side="right", pady=4)

        cards = tk.Frame(top_container, bg=BG)
        cards.pack(fill="x")

        def make_card(icon, title, subtitle, handler, extra=None):
            card = ResourceCard(cards, icon, title, subtitle, handler, extra)
            card.pack(fill="x", pady=5)
            return card

        self.client_row = make_card(
            "◆", "Client Binary",
            "Game####.swf  ·  gameversion API with automatic retry guard",
            self.fetch_client,
        )

        def assets_extra(parent):
            f = tk.Frame(parent, bg=CARD)
            tk.Label(f, text="DATE (BLANK = AUTO)", font=("Consolas", 7, "bold"), bg=CARD, fg=ACCENT).pack(side="left")
            entry = tk.Entry(
                f, textvariable=self.assets_date_var, width=12,
                bg=PANEL, fg=FG, insertbackground=FG,
                relief="flat", font=FONT_MONO,
                highlightthickness=1, highlightbackground=BORDER,
                highlightcolor=ACCENT,
            )
            entry.pack(side="left", padx=(8, 0), ipady=2)
            return f

        self.assets_row = make_card(
            "◇", "Game Assets Pack",
            "Assets_YYYYMMDD.swf  ·  leaves blank for intelligent auto-detection",
            self.fetch_assets, extra=assets_extra,
        )

        self.version_row = make_card(
            "▣", "Game Version Meta",
            "JSON telemetry metadata  ·  build identifiers & active pointer",
            self.fetch_version,
        )

        self.servers_row = make_card(
            "☰", "Server Status Hub",
            "JSON server array  ·  live population density diagnostics",
            self.fetch_servers,
        )

        self.rows = [
            self.client_row, self.assets_row,
            self.version_row, self.servers_row,
        ]

        log_wrap = tk.Frame(self, bg=PANEL, highlightthickness=1,
                            highlightbackground=BORDER)
        log_wrap.grid(row=4, column=0, sticky="nsew", padx=20, pady=(6, 0))
        log_wrap.rowconfigure(1, weight=1)
        log_wrap.columnconfigure(0, weight=1)

        log_hdr = tk.Frame(log_wrap, bg=PANEL)
        log_hdr.grid(row=0, column=0, sticky="ew", padx=10, pady=(6, 0))
        tk.Label(log_hdr, text="SYSTEM TERMINAL // LIVE TELEMETRY LOG", font=("Consolas", 8, "bold"), bg=PANEL, fg=ACCENT).pack(side="left")

        self.log_box = scrolledtext.ScrolledText(
            log_wrap, bg=PANEL, fg=FG, insertbackground=FG,
            font=FONT_MONO, wrap="word", borderwidth=0,
            highlightthickness=0, state="disabled",
            padx=8, pady=6,
        )
        self.log_box.grid(row=1, column=0, sticky="nsew", padx=4, pady=4)

        for tag, color in (
            ("ok", GREEN), ("warn", YELLOW), ("error", RED),
            ("info", DIM), ("phase", ACCENT), ("time", DIM),
        ):
            self.log_box.tag_configure(tag, foreground=color)

        bar = tk.Frame(self, bg=BG)
        bar.grid(row=5, column=0, sticky="ew", padx=20, pady=16)

        self.cancel_btn = RoundedButton(
            bar, text="ABORT", command=self.cancel_fetch,
            bg_color=RED, hover_color="#f43f5e", fg_color="#06080c",
            width=90, height=32, radius=7, font=FONT_TINY
        )
        self.cancel_btn.pack(side="left")
        self.cancel_btn.configure_state("disabled")

        self.open_btn = RoundedButton(
            bar, text="OPEN DIRECTORY", command=self.open_folder,
            bg_color=CARD, hover_color=CARD_HOVER, fg_color=FG,
            width=125, height=32, radius=7, font=FONT_TINY
        )
        self.open_btn.pack(side="left", padx=(8, 0))

        self.clear_btn = RoundedButton(
            bar, text="CLEAR LOG", command=self.clear_log,
            bg_color=CARD, hover_color=CARD_HOVER, fg_color=FG,
            width=100, height=32, radius=7, font=FONT_TINY
        )
        self.clear_btn.pack(side="left", padx=(8, 0))

        tk.Label(bar, text="TARGET DIR: SCRIPT ROOT",
                 font=("Consolas", 7), bg=BG, fg=DIM).pack(side="right")

    def append_log(self, msg, tag="info"):
        self.log_box.configure(state="normal")
        ts = datetime.now().strftime("%H:%M:%S")
        prefix = {
            "ok": "  [OK]  ", "warn": "  [WRN] ", "error": "  [ERR] ",
            "phase": ">> ", "info": "        ",
        }.get(tag, "        ")
        if tag == "phase":
            self.log_box.insert("end", f"\n[{ts}] ", "time")
            self.log_box.insert("end", f"{prefix}{msg}\n", tag)
        else:
            self.log_box.insert("end", f"[{ts}] ", "time")
            self.log_box.insert("end", f"{prefix}{msg}\n", tag)
        self.log_box.see("end")
        self.log_box.configure(state="disabled")

    def _set_busy(self, busy):
        for r in self.rows:
            r.set_busy(busy)
        self.all_btn.configure_state("disabled" if busy else "normal")
        self.cancel_btn.configure_state("normal" if busy else "disabled")

    def _start_job(self, row, target_name, *args):
        if self.worker and self.worker.is_alive():
            return
        self.active_row = row
        self.progress_var.set(0)
        self.status_var.set("STATUS: INITIALIZING TASK...")
        if row:
            row.set_status("PROCESSING...")
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
        self._start_job(row, "run_assets", date_str)

    def fetch_version(self, row):
        self._start_job(row, "run_gameversion")

    def fetch_servers(self, row):
        self._start_job(row, "run_servers")

    def fetch_all(self):
        date_str = self.assets_date_var.get().strip()
        self._start_job(None, "run_all", date_str)

    def cancel_fetch(self):
        if self.fetcher:
            self.fetcher.cancel_flag.set()
        self.status_var.set("STATUS: ABORTING SEQUENCE...")

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

    def _poll_queue(self):
        try:
            while True:
                kind, payload = self.msg_queue.get_nowait()
                if kind == "log":
                    self.append_log(payload["msg"], payload.get("level", "info"))
                elif kind == "status":
                    self.status_var.set(f"STATUS: {payload['msg'].upper()}")
                    if self.active_row:
                        self.active_row.set_status(payload["msg"][:18])
                elif kind == "phase":
                    self.append_log(payload["title"], "phase")
                elif kind == "progress":
                    self.progress_var.set(payload["pct"])
                elif kind == "all_step":
                    i, t = payload["index"], payload["total"]
                    self.status_var.set(f"STATUS: SEQUENCE STEP {i}/{t} IN PROGRESS")
                elif kind == "done":
                    if payload.get("ok"):
                        self.status_var.set(
                            f"STATUS: COMPLETE // {payload.get('summary', '').upper()}"
                        )
                        dest = payload.get("dest")
                        if dest:
                            self.last_dest_dir = (
                                dest if os.path.isdir(dest)
                                else os.path.dirname(dest)
                            )
                    else:
                        self.status_var.set("STATUS: ERROR ENCOUNTERED (SEE LOG)")
                    self._job_finished()
        except queue.Empty:
            pass
        self.after(60, self._poll_queue)


if __name__ == "__main__":
    App().mainloop()