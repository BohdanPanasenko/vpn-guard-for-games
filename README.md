# vpn-guard-for-games

Reminds you (or makes you) to turn off your VPN before launching a game that doesn't work properly over a VPN (usually an online game).

I decided to create this program for myself for Hunt: Showdown + NordLayer on Windows, but it’s designed to work with any game on Steam (and actually non-Steam game as well). 

Why did I do it? Because I’m an idiot who constantly forgets to turn off my work VPN after my shift. As a result, I either end up playing with a ping over 200 ms, or I turn it off, - but then I have to restart the game, which is annoying. So yeah, here we are.

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
