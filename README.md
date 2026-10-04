# vpn-guard-for-games

Reminds you (or makes you) turn off your VPN before launching a game that misbehaves behind it.

I decided to build for Hunt: Showdown + NordLayer on Windows, but designed to work with any Steam game.
Why? Because I am an idiot who keeps forgetting to turn off corporate VPN after a work day. In result I am either playing with 200+ ms ping or turn it off, but then have to relaunch the game. 

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
