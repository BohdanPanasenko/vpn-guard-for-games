# vpn-guard-for-games

Reminds you (or makes you) turn off your VPN before launching a game that misbehaves behind it.

I decided to build for Hunt: Showdown + NordLayer on Windows, but designed to work with any Steam game.
Why? Because I am an idiot who keeps forgetting to turn off corporate VPN after a work day. In result I am either playing with 200+ ms ping or turn it off, but then have to relaunch the game. 

## Setup

Run `build.cmd`. It compiles `vpnguard.exe` with the C# compiler that ships with Windows (nothing to install) and copies it to `%LOCALAPPDATA%\vpn-guard-for-games\`, a fixed location that doesn't depend on where this repo lives.

Then, in the game's **Steam > Properties > Launch Options**:

```
"C:\Users\<you>\AppData\Local\vpn-guard-for-games\vpnguard.exe" %command%
```

`build.cmd` prints the exact line for your machine. Double-clicking `vpnguard.exe` shows whether the VPN is currently detected.

## How it works

When you press Play:

1. vpnguard checks whether the VPN is connected (any network adapter with "NordLayer" in its name that is up).
2. If it is not, the game starts immediately.
3. If it is, a dialog offers **Open NordLayer** / **Play anyway** / **Cancel**. As soon as you disconnect, the dialog closes and the game starts on its own.

vpnguard stays running until the game exits, so Steam still tracks playtime correctly.

The same line works for any Steam game (Dota 2, Rainbow Six Siege, ...). For non-Steam games, point a desktop shortcut at `vpnguard.exe "C:\path\to\game.exe"`.

## Roadmap

- [ ] v1: detect VPN + prompt, then launch the game
- [ ] Config file: per-mode behaviour (always ask / silent)
- [ ] v2: automatic disconnect (NordLayer has no CLI; needs investigation)
- [ ] Offer to reconnect after the game exits
- [ ] Background watcher mode for non-Steam games
