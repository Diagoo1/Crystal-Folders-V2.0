<p align="center"></p>
<p align="center"><a href="#"><img width="115px" src="https://raw.githubusercontent.com/Diagoo1/Crystal-Folders-V2.0/main/CrystalFolders/Resources/Icon - Copy.png" align="center" alt="Crystal Folders"/></a></p>
<h1 align="center">Crystal Folders v2.0</h1>
<p align="center"><b>Crystal Folders</b> is a modern Windows application reimagined to customize and colorize your folder icons in seconds. Now featuring a brand-new UI, dynamic themes, and native Arabic support.</p>

<p align="center">
 <a href="LICENSE"><img alt="License" src="https://img.shields.io/badge/License-MIT-0984E3?style=flat-square&labelColor=343B45"/></a>
 <a href="https://github.com/Diagoo1/Crystal-Folders-V2.0/releases/latest"><img src="https://img.shields.io/github/v/release/Diagoo1/Crystal-Folders-V2.0.svg?color=0984E3&label=Release&style=flat-square&labelColor=343B45"/></a>
 <a href="#"><img alt="NET" src="https://img.shields.io/badge/.NET_Framework-4.8-0984E3?style=flat-square&labelColor=343B45"/></a> 
 <a href="#"><img alt="Languages" src="https://img.shields.io/badge/Languages-2-0984E3?style=flat-square&labelColor=343B45"/></a>
</p>

<p align="center">
<a href="README.md">English</a> :speech_balloon: <a href="README-ar.md">العربية</a>
</p>

---

## 🌟 The Reimagined Experience (v2.0.0)
This version is a **major milestone** and a complete overhaul from the ground up. We’ve redesigned everything to provide a premium customization experience:
* **🎨 Brand New UI:** A modern, sleek design inspired by Windows 11 with smooth animations and adaptive rounded corners.
* **🌙 Native Dark Mode:** Full integration for a comfortable visual experience during night sessions.
* **🖼️ Image to Icon Engine:** Directly convert your favorite images into high-quality folder icons within the app.
* **🌍 Full Arabic Support:** Complete RTL (Right-to-Left) localization for Arabic speakers.
* **🌈 Dynamic Accent Colors:** Pick your own personal color to theme the entire application interface.

---

## 🚀 Key Features
* ⚡ **Lightning Fast:** Change hundreds of icons in a heartbeat.
* 🖱️ **Drag & Drop:** The simplest way to add folders to your list.
* 📂 **One-Click Libraries:** Quick access to Documents, Pictures, Music, and Videos.
* 🔄 **Subfolder Customization:** Depth-first icon replacement for all nested directories.
* 📱 **Portable Mode:** Icons stay visible even if you move the folder to another PC. [𝐢](#details)
* 🔔 **Desktop Notifications:** Get notified immediately when the customization is complete.
* 📑 **Metadata Safe:** Modifies `Desktop.ini` without losing your existing folder information.

---

## 📸 Preview
<a href="#"><img src="docs/assets/Crystal-Folders.gif"/></a>

---

## 📖 How to Use
1. **📥 Add Folders:** Use the side checkboxes or simply drag and drop folders into the workspace.
2. **🎨 Choose Your Icon:** Pick from 7 premium default colors or use the **Image-to-Icon** tool for a custom touch.
3. **⚙️ Personalize:** Click the **Info (i)** button to toggle **Dark Mode**, change **Language**, or pick a new **Accent Color**.
4. **✅ Apply:** Hit `Customize` and wait for the smooth transition window and confirmation.

---

<br id="details"/>

### 📱 Portable Folders (Pro Feature)
The `Configure to portable` switch allows you to customize the icon for up to **30 folders** so they appear customized on any other computer. It stores the icon file directly inside the folder for maximum compatibility.

---

## 🛡️ Security Measures
* 🔒 **System Protection:** Special folders (like Documents/Pictures) are protected to prevent system errors.
* 🚫 **Privilege Check:** Automatically skips folders that require higher administrative permissions.
* ⚠️ **Limit Alert:** A safety warning appears if you try to process more than **600 folders** (Adjustable in config).

---

## ⚙️ Smart Configuration (`Config.ini`)
To keep your system clean, your settings are **no longer stored next to the executable**. They are now safely managed in the Windows AppData directory:
* **Path:** `%AppData%\CrystalFolders\Config.ini`

---

## 🆕 What's New? (Version 2.0.0)
* **Major:** Full UI redesign with modern Windows 11 aesthetics.
* **New:** Native Arabic Translation & RTL Layout support.
* **New:** Custom Color Picker for UI (Accent Colors).
* **New:** Persistent Dark Mode across sessions.
* **Improved:** Migration of config files to the secure `AppData` folder.
* **Fixed:** HandyControl UI resource loading and scaling bugs.

---

## 🛠️ Build & Installation
* **Source:** Compiled via **Visual Studio** (.NET Framework 4.8).
* **Installer:** Built with [Inno Setup](https://jrsoftware.org/isinfo.php). Files in [/installer src](/installer%20src).

---

## 📜 Credits & Acknowledgments

### 👨‍💻 Development Team
* **Tarek:** Lead developer of the reimagined version (v2.0.0). Implemented Arabic localization, Dynamic Themes, Dark Mode, and core system optimizations.
* **[Génesis Toxical](https://github.com/genesistoxical):** Original creator of the first version of Crystal Folders. Thank you for the solid foundation.

### 📚 Third-Party Libraries
* [HandyControls](https://github.com/ghost1372/HandyControls) - Modern UI components under MIT License.
* [FolderBrowserEx](https://github.com/evaristocuesta/FolderBrowserEx) - Enhanced selection dialogs under MIT License.
* [Noto Music](https://fonts.google.com/noto/specimen/Noto+Music) - UI Fonts under SIL Open Font License.

---

## ⚖️ License
**MIT License** Copyright (c) 2025 - 2026 **Crystal Folders Project** - Maintained by Tarek.
