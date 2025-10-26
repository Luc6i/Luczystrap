# Luczystrap

> A modern, community-first Roblox bootstrapper — lean like Bloxstrap, polished like Fishstrap, built to outclass every fork.

<p align="center">
  <a href="https://github.com/Luc6i/Luczystrap/actions">
    <img alt="CI" src="https://img.shields.io/github/actions/workflow/status/Luc6i/Luczystrap/ci.yml?label=build">
  </a>
  <a href="https://github.com/Luc6i/Luczystrap/releases">
    <img alt="Releases" src="https://img.shields.io/github/v/release/Luc6i/Luczystrap?display_name=tag&sort=semver">
  </a>
  <a href="https://github.com/Luc6i/Luczystrap/releases">
    <img alt="Downloads" src="https://img.shields.io/github/downloads/Luc6i/Luczystrap/total">
  </a>
  <a href="https://discord.gg/C2rkCMkc9c">
    <img alt="Discord" src="https://img.shields.io/discord/1429853228673007747?logo=discord&label=discord">
  </a>
  <a href="#license">
    <img alt="License" src="https://img.shields.io/badge/license-MIT-blue">
  </a>
</p>

Luczystrap is a clean, fast, and extensible alternative Roblox bootstrapper. It ships with the **core feature set you already know from Bloxstrap & Fishstrap**, and is designed to grow into the most trusted, best-documented bootstrapper in the ecosystem.

---

## ✨ Key Features

- **FFlag editor** — search, toggle, and persist flags quickly.
- **Themes & appearance** — curated presets plus fine-grained controls.
- **Mods & assets** — drop-in sounds, cursors, textures, and UI tweaks.
- **Profiles** — group your flags, theme, and mods per game or context.
- **Discord Rich Presence** — presence that just works, with smart fallbacks.
- **Portable by default** — offline-friendly; no .ROBLOSECURITY needed for features.

> Roadmap items like delta updates, self-repair, and a CLI are tracked as milestones.

---

## 🚀 Quick Start

1. **Download** the latest release from the [Releases page](https://github.com/Luc6i/Luczystrap/releases).
2. **Run** `Luczystrap.exe` once — it will detect Roblox and set itself up.
3. Open **Flags**, **Themes**, or **Mods** to customize. Create a **Profile** to save your full setup.

### Optional hardening

- **Settings → Integrity**
  - *Lock my flags* to prevent accidental edits.
  - *Validate install on launch* to hash-check core files.

---

## 🧩 Features — Now vs. Next

| Area | Current parity (Bloxstrap/Fishstrap) | Luczystrap advantage | Status |
|---|---|---|---|
| FastFlags | Search/filter, set & persist | Prefix search, conflict hints, per-profile overrides | ✅ |
| Themes | Light/Dark + custom palettes | One-click presets, live preview, fine sliders | ✅ |
| Mods | Sounds, cursors, textures | Safe mod folders, per-profile mod sets | ✅ |
| Discord RPC | Basic presence | Rule-based presence composer, fallback if Discord closed | 🔜 |
| Profiles | — | Full profiles (Flags + Theme + Mods) | 🔜 |
| Updates | Manual download | Delta/patch updates, rollback, offline installers | 🔜 |
| Integrity | — | Hash verify, tamper hints, repair tool | 🔜 |
| Automation | — | Headless CLI for profiles & flags | 🔜 |
| Cloud Sync | — | Opt-in export/import to JSON/Gist | 🔜 |

Legend: ✅ = available · 🔜 = planned

---

## 📦 Install / Portable

- **Portable:** place `Luczystrap.exe` anywhere; config lives beside the app.
- **Installed:** standard user-profile paths are used.

**Config paths**  
- Portable: `./Luczystrap/UserData/`  
- Installed: `%AppData%\Luczystrap\`  
- Logs: `%LocalAppData%\Luczystrap\logs\`

---

## 🛣️ Roadmap (milestones)

1. **M0 — Public build**: parity with core features; stable installer + portable zip.  
2. **M1 — Profiles & Presets**: per-game profiles, export/import, curated theme & flag presets.  
3. **M2 — Integrity & Repair**: file hashes, self-repair, safe-mode launch, diagnostic bundle.  
4. **M3 — Updates & CLI**: delta auto-updates with rollback; a `luczystrap` CLI.  
5. **M4 — Presence Composer**: rule-based Discord RPC with privacy filters.

---

## 📂 Repository Structure (simplified)

Luczystrap/
├─ src/
│ ├─ Luczystrap.App/ # UI / bootstrapper
│ ├─ Luczystrap.Core/ # Flags, profiles, integrity
│ └─ Luczystrap.CLI/ # Command-line tooling (planned)
├─ mods/ # Sample mods & templates
├─ docs/ # Guides, presets, FAQ
└─ .github/
├─ ISSUE_TEMPLATE/
└─ workflows/


---

## 🧠 Design Principles

- **Trust first** — transparent code paths; minimal external calls.
- **Predictable UX** — changes are visible and reversible; profiles keep states explicit.
- **Portable** — works offline; settings travel with the app when portable.
- **Extensible** — future plugin points guarded by integrity checks.

---

## 🐛 Troubleshooting

- **Roblox launches without changes**: ensure your **Profile** is active and your flags file has a recent timestamp.  
- **Discord presence not showing**: verify Discord is running; presence composer will add robust fallbacks (planned).  
- **Game broke after a mod**: switch to **Safe Mode** (planned) or temporarily rename the `Mods` folder.

When filing an issue, attach your log bundle from **Settings → Diagnostics → Create bundle**.

---

## 🤝 Contributing

See [CONTRIBUTING.md](./CONTRIBUTING.md). Before opening a PR, prefer an Issue with context, and keep changes small and testable. A clear rationale beats a big diff.

- Code of Conduct: [CODE_OF_CONDUCT.md](./CODE_OF_CONDUCT.md)  
- Security policy: [SECURITY.md](./SECURITY.md)  
- Changelog: [CHANGELOG.md](./CHANGELOG.md)

---

## 🔐 Security & Privacy

- **Privacy:** offline-first. No analytics or tracking by default.  
- **Network calls:** updates (opt-in when available) and optional cloud sync only.  
- **Code signing:** targeted for stable releases.

---

## 💬 Community & Support

Join the official **Luczystrap Discord** to get updates, discuss development, and share mods:

[![Discord](https://img.shields.io/discord/0?logo=discord&label=Join%20the%20community)](https://discord.gg/C2rkCMkc9c)

---

## ❤️ Credits

**Luczystrap** is built and maintained by **[Luci](https://github.com/Luc6i)**  
with major contributions, ideas, and testing support from **Midas** and the broader community.

Special thanks to open-source bootstrapper projects whose ideas and groundwork inspired Luczystrap:

- [Bloxstrap](https://github.com/bloxstraplabs/bloxstrap)  
- [Fishstrap](https://github.com/fishstrap/fishstrap)

Luczystrap continues their legacy — rebuilt with cleaner code, modular architecture, and stronger user safety.

---

## 👨‍💻 Developers

<table>
  <tr>
    <td align="center">
      <a href="https://github.com/Luc6i">
        <img src="https://avatars.githubusercontent.com/u/your_user_id_here?v=4" width="100px;" alt="Luci"/><br />
        <sub><b>Luci</b></sub>
      </a><br />
      💡 Founder / Lead Developer
    </td>
    <td align="center">
      <a href="https://github.com/Midaskira">
        <img src="https://avatars.githubusercontent.com/u/your_user_id_here?v=4" width="100px;" alt="Midaskira"/><br />
        <sub><b>Midaskira</b></sub>
      </a><br />
      🧠 Co-Developer / UI & Testing
    </td>
  </tr>
</table>

---

## 📜 License

This project is licensed under the **MIT License**. See [LICENSE](./LICENSE).

---

## ⭐ One-liner Mission

**Make Roblox customization effortless, safe, and beautifully documented — so players choose Luczystrap without thinking twice.**
