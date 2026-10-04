# vpn-guard-for-games

Reminds you (or makes you) to turn off your VPN before launching a game that doesn't work properly over a VPN (usually an online game).

I decided to create this program for myself for Hunt: Showdown + NordLayer on Windows, but it’s designed to work with any game on Steam (and actually non-Steam game as well). 
Why did I do it? Because I’m an idiot who constantly forgets to turn off my work VPN after my shift. As a result, I either end up playing with a ping over 200 ms, or I turn it off, - but then I have to restart the game, which is annoying. So yeah, here we are.

## How it works

vpn-guard-for-games runs as a Steam launch-option wrapper. In the game's **Properties > Launch Options**:

```
"E:\vpn-guard-for-games\vpn-guard-for-games.exe" %command%
```

When you press Play:

1. vpn-guard-for-games checks whether the VPN is connected.
2. If it is not, the game starts immediately.
3. If it is, a dialog offers: **Disconnect & play** / **Play anyway** / **Cancel**.

The same line works for any Steam game (Dota 2, Rainbow Six Siege, ...).

## Roadmap

- [ ] v1: detect VPN + prompt, then launch the game
- [ ] Config file: per-mode behaviour (always ask / silent)
- [ ] v2: automatic disconnect (NordLayer has no CLI; needs investigation)
- [ ] Offer to reconnect after the game exits
- [ ] Background watcher mode for non-Steam games
