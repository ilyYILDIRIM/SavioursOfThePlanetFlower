# Saviours of The Planet Flower

A Space Invaders-style wave defense game built with MonoGame (.NET 9).

---

## Objective

Defend the **Flower Planet** at the bottom of the screen from enemy invasion.
The game ends only when the planet's HP reaches zero.
Survive all **15 waves** to win.

---

## Controls

| Key / Input | Action |
|---|---|
| `A` / `←` | Move left |
| `D` / `→` | Move right |
| `Space` / `Left Click` | Shoot |
| `E` | Activate Reflect ability (if unlocked) |
| `Escape` | Pause / Unpause |

---

## Enemies

| Enemy | Description |
|---|---|
| **Normal Enemy** | Moves left and right, steps down toward the planet. Does not shoot. |
| **Shooting Enemy** | Same movement as normal, but fires projectiles downward. |
| **Boss** (every 5th wave) | A full-width bar at the top. Has 2000 HP. Each hit deals only 5 damage. Fires random-position projectiles that deal 50 damage to the planet and instantly kill the player and turrets. |

---

## Turrets

Three/four auto-firing turrets are placed between the player and the planet.
Each turret has its own HP bar displayed above it.
Turrets automatically shoot upward every 1.5 seconds.

---

## Player Death

When the player dies, the game **does not end**.
Turrets continue fighting automatically.
Game speed increases to **3x** while the player is dead.
The player respawns at the start of the next wave.

---

## Wave System

- Each wave is cleared when all enemies (or the boss) are defeated.
- After each wave, an **Upgrade Shop** appears before the next wave begins.
- Every 5th wave is a Boss wave.
- After wave 15, you win.

---

## Upgrade Shop

Spend your score points on upgrades between waves:

| Upgrade | Effect | Cost |
|---|---|---|
| Player Speed | +50 move speed | 20 pts |
| Player Fire Rate | Faster shooting | 25 pts |
| Player HP | +1 health | 30 pts |
| FlowerPlanet HP | +20 max HP | 25 pts |
| Turret HP | +2 health (per turret) | 20 pts |
| Turret Fire Rate | Faster shooting (per turret) | 25 pts |
| Turret Revive | Revive a destroyed turret | 50 pts |
| Reflect Ability | Unlock the Reflect skill | 60 pts |

---

## Reflect Ability

Once unlocked, press `E` to activate Reflect for **10 seconds**.
While active, enemy projectiles (including boss bullets) that hit the player are reflected back as friendly projectiles.
Reflect has a **30 second cooldown** after use.

---

## Scoring

- Each enemy killed: **+10 points**
- Score is saved to `highscore.json` after each wave clear or game over.

---

## Game Over Conditions

- The **Flower Planet's HP reaches 0** (from enemy projectiles or boss shots).
- Enemies reach **65% of the screen height** without being stopped.

---

## Built With

- MonoGame Framework (DesktopGL)
- .NET 9
- C# — OOP, Interfaces, Generics, LINQ, Async/Await, Observer Pattern, Serialization
