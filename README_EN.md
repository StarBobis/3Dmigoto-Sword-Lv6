
# ⚔️ 3Dmigoto-Sword-Lv6

> 🧰 **3Dmigoto-Sword-Lv6** is a tool for **extracting and converting models** from 3Dmigoto-format Mods into `.fmt`, `.ib`, and `.vb` formats.
> Once converted, these files can be imported into **Blender** using the **TheHerta3 plugin**.
>
> The tool also provides several additional utilities commonly used by Mod creators.

---

## 🪄 Purpose of the Tool

* 🧩 **Repairing broken or outdated Mods**
  Fix Mods that were damaged after a game update due to changes in weights or vertex group indices.
  This requires extracting the model from the Mod first.

* 🎓 **Learning modeling techniques**
  Many Mods contain valuable modeling details — such as topology, edge flow, and UV layouts — that can be studied and appreciated.

* 📦 **Extracting models from Mods**
  The tool itself only extracts models.
  It does **not** define or authorize any specific use for the extracted assets.

---

## 🚫 Forbidden Uses

* ❌ **Modifying others’ Mods and releasing them as your own**
  This violates the original creator’s right to control and modify their work, potentially harming community harmony and authors’ earnings.

* 💰 **Reselling extracted models**
  Doing so infringes upon Mod creators’ legitimate economic interests.

* ⚖️ **Using this tool to violate Mod creators’ rights**
  Always respect copyright notices and usage permissions included with Mods.

---

## ⚖️ Controversy Notice

This tool has been somewhat controversial within the community.
Some **AGMG community** Mod authors oppose it, believing it may harm creators’ interests.
👉 **Please refrain from promoting or sharing this tool within AGMG.**

> 💡 The original purpose of this project was **to repair old Mods**.
> Over time, it evolved into a **general-purpose 3Dmigoto Mod extraction tool**.

If you have negative opinions about the tool,
please direct criticism toward **those who misuse it**, not its developer. 🙏

---

## 🔒 About Partial Closed Source

* The **GUI** is developed in **C# (WinUI3)** — *open source ✅*
* Certain core functions rely on **C++ plugins** — *closed source 🔐* (e.g. `3Dmigoto-Sword-Lv5.exe`)
* The tool also uses **`texconv.exe`** from Microsoft’s **DirectXTK** project for texture format conversion.

> 📘 The GUI is licensed under **GPL 3.0**.
> However, the plugin remains closed source due to its advanced automation and reverse-engineering features, which are considered controversial in the Modding community.

---

## 🔌 Plugin Access

While the GUI can run independently for some features (like **manual reverse-engineering**),
certain advanced functions require the **3Dmigoto-Sword-Lv5.exe** plugin.

Data structures for manual reverse-engineering are based on **SSMT3**,
which you can freely extend or edit for custom workflows.

🔗 **Plugin access** is limited to members of the **SSMT technical community**.
For details, see the following documentation:

📚 **Yuque Docs**
👉 [SSMT Reverse Plugin Introduction](https://www.yuque.com/airde/brypfn/lg0xv72f0lxzsqut?singleDoc#)

📘 **GitHub Docs**
👉 [SSMT-Documents Reverse Plugin Guide](https://starbobis.github.io/SSMT-Documents/Tutorials/Plugins/SSMT-Reverse/%281%29SSMT-Reverse%E6%8F%92%E4%BB%B6%E4%BB%8B%E7%BB%8D/%281%29SSMT-Reverse%E6%8F%92%E4%BB%B6%E4%BB%8B%E7%BB%8D.html)

💖 **Support the project**
👉 [Afdian Donation Page](https://afdian.com/item/ec74ee782b2f11efb5a052540025c377)

---

## 🧱 Plugin Installation

1. After downloading the plugin, click **“Open Assets Folder”** inside the app.
2. Place the plugin file in that folder — it will automatically be detected and enabled ✅

---

## 🩸 The Spirit of Noxus

> The development philosophy behind this tool is inspired by the **Noxus Empire** from *League of Legends*.
> Its icon uses the **Noxus emblem**, symbolizing strength and determination.

---

### 🏛️ 1. In the World of *League of Legends*

**Noxus** represents *power, conquest, and pragmatism*.
Its creed is simple yet profound:

> 💬 “Strength deserves respect — and strength can come from anywhere.”

**Core idea:**

> No matter your origin, if you have strength, you earn respect and position.

**Notable figures:**

* 🩸 **Darius** — Symbol of rising to power through sheer strength
* 🧠 **Swain** — Mastermind representing intellect and strategy as forms of power
* ⚔️ **Katarina** — Embodying precision, loyalty, and dedication to Noxian ideals

---

### 🌍 2. Cultural Interpretation (Real-World Analogy)

The **Noxus philosophy** parallels a form of **extreme meritocracy** —
a worldview where **results and capability** outweigh **background and sentiment**.

| 🌟 Positive Aspects                           | ⚠️ Negative Aspects                   |
| --------------------------------------------- | ------------------------------------- |
| Encourages effort and equality of opportunity | Can lead to cruelty and indifference  |
| Respects achievement and competence           | May ignore fairness or compassion     |
| Focuses on efficiency and results             | Risks losing human empathy and ethics |

It’s both **inspiring and dangerous** — a philosophy of *ambition, strength, and consequence*.
It asks one fundamental question:

> 🧩 “When power and morality collide, which would you choose?”

---

### 🩸 3. Summary

> **The Spirit of Noxus = A creed of power-centered pragmatism.**
>
> 💬 *“Regardless of origin, background, or method — if you possess true strength, you can change the world.”*

