# 🎵 Music Store Showcase (Fake Data Generator)

## 📌 Overview

This project is a **single-page web application** that simulates a music store by generating realistic (but fake) song data, including audio previews.

The application focuses on:

* Deterministic random data generation
* Dynamic UI updates
* Localization support
* Reproducibility via seeds

---

## 🚀 Features

### 🌍 Language Selection

* Supports **English (USA)** (required)
* Includes at least one additional locale (e.g., German, Ukrainian)
* Each language generates **independent datasets**

---

### 🎲 Seed Configuration

* Custom **64-bit seed input**
* Option to generate a **random seed**
* Ensures full reproducibility:

  * Same seed → same data (across devices and time)

---

### ❤️ Likes Per Song

* Adjustable average: **0–10**
* Supports fractional values (e.g., `3.7`)
* Implemented probabilistically:

  * Example:

    * `0` → no likes
    * `0.5` → ~50% chance of 1 like
    * `10` → always 10 likes

---

## 🖥️ UI / UX

### 🔧 Toolbar

All controls are placed in a **single horizontal toolbar**:

* Language
* Seed
* Likes

### ⚡ Dynamic Updates

* Data updates instantly when parameters change
* No buttons or Enter required

---

### 📊 Display Modes

#### Table View

* Pagination enabled
* Expandable rows
* Detailed view includes:

  * Album cover
  * Play button
  * Reviews

#### 🖼️ Gallery View

* Infinite scrolling
* Loads data in batches ("pages")

---

### 🔄 Reset Behavior

Changing parameters:

* Resets table to page 1
* Resets gallery scroll

---

## 🎼 Generated Data

Each item includes:

* Index (1, 2, 3, …)
* Song title
* Artist (band or person)
* Album (or `"Single"`)
* Genre

---

## 🔐 Authentication

❌ No authentication required

---

## 🌐 Localization Rules

* All generated content matches selected locale
* Data should look realistic (but may be nonsensical)
* ❌ No placeholder text like lorem ipsum

---

## ⚙️ Parameter Independence

* All parameters are independent:

  * Changing **likes** → updates only likes
  * Changing **seed/region** → regenerates all content

---

## 🎧 Song Generation

* Real audio generated in-browser
* Deterministic:

  * Same seed → same sound output

---

## 🧪 Optional Features

* Export songs as **ZIP (MP3 files)**
* Lyrics with **synchronized scrolling**

---

## 🏗️ Architecture

### Backend

* All data generated **server-side**
* No persistent storage required
* Server returns one "page" at a time

### Database

* ❌ Not required for generated data
* May be used for localization data (optional)

---

### 🔁 Seed Logic

* RNG seed = `userSeed + pageNumber` (or similar)
* Ensures:

  * Reproducibility
  * Variation across pages

---

### 📦 Code Requirements

* Use libraries:

  * Faker (or equivalent) for data
  * Music generation tools
* Avoid hardcoded locale data (except temporary shortcuts)

---

## 🎯 Notes

* Data should feel realistic and visually appealing
* Music should resemble structured melodies (not noise)
* Advanced implementations may include:

  * Chord progressions
  * Multiple instruments
  * Effects (reverb, echo)

---


## 💡 Summary

This project combines:

* Procedural generation
* UI/UX responsiveness
* Localization
* Deterministic randomness

A solid exercise in building **interactive, scalable, and reproducible web applications**.
