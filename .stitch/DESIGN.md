# Design System: Ghost Optimizer v2.0
**Project ID:** Local / Promo Page

## 1. Visual Theme & Atmosphere
The visual theme is a high-performance, dark-mode, gamer-centric interface with a clean and modern tech aesthetic. It uses a Cyberpunk/Futuristic atmosphere characterized by dark backgrounds, neon-glow backdrops, and semi-transparent glassmorphic panels. The overall mood is immersive, sleek, and high-tech, projecting reliability, speed, and cutting-edge software optimization capabilities.

Key visual features include:
- A dark grid backdrop with ultra-subtle borders representing precise matrix grids.
- Diffused glowing elements in the corners utilizing vibrant cyan and deep purple gradients to create visual depth.
- Glassmorphic containers with blur backdrops and clean, hairline-thin borders to separate content without feeling cluttered.

## 2. Color Palette & Roles
- **Vibrant Electric Cyan (`#00d9ff`):** Used as the primary accent color. It highlights active statuses, core metrics, key brand markers, and interactive links. It conveys high speed, latency reduction, and active optimization.
- **Deep Muted Amethyst Purple (`#a970ff`):** Used as the secondary accent color. It is combined with Cyan in gradients (such as badges and main headers) to denote premium performance and advanced power.
- **Blurple Discord Blue (`#5865f2`):** Used for primary UI selectors (e.g., active navigation buttons and profile cards) to provide clear contrast against dark surfaces.
- **Vibrant Emerald Green (`#10b981` / `#0e7a0d`):** Used as a status indicator for "Safe" tweaks and the active "ON" toggle states. It denotes optimized and active performance.
- **Glowing Amber Yellow (`#f59e0b`):** Used as a status indicator for "Moderate" risk level tweaks.
- **Vibrant Ruby Red (`#ef4444` / `#a80000`):** Used as a status indicator for "Danger/Risky" tweaks and inactive "OFF" states.
- **Deep Space Black (`#0a0a0c`):** The primary background color. Provides an ultra-dark canvas that makes neon elements pop.
- **Midnight Navy-Grey (`#0f0f13`):** The secondary background color, used to differentiate major page panels and content zones.
- **Dark Slate Slate-Grey (`#131418` / `#181920`):** Used for sidebar navigation, card containers, and headers to create hierarchical layering.
- **Clean Ice White (`#ffffff`):** Used for primary body text, titles, and high-priority labels.
- **Soft Cloud White (`#e2e8f0`):** Used for secondary text, descriptions, and list item content.
- **Muted Cool Grey (`#94a3b8`):** Used for inactive links, unit labels, and captions.

## 3. Typography Rules
- **Header Typeface:** `Outfit` (Geometric, tech-oriented sans-serif). Used exclusively for page titles, main headings, badges, and hero titles. It features bold weights (up to `800` or Extra Bold) and is frequently applied with gradients (Cyan-to-Purple) to make technical copy visually striking.
- **Body Typeface:** `Inter` (Clean, neutral sans-serif). Used for all body text, tweak list labels, stats, button labels, descriptions, and terminal logs. It uses weights of `300` (Light), `400` (Regular), `500` (Medium), and `600` (Semi-Bold) to establish clear reading hierarchy.
- **Letter Spacing:** Titles and badges feature expanded tracking (letter-spacing of `1px` to `2px`) to enhance their technical and structural character.

## 4. Component Stylings
* **Buttons:**
  - *Primary Call-to-Action:* Rectangular buttons with subtly rounded corners (`border-radius: 8px`), filled with Vibrant Electric Cyan (`#00d9ff`) and featuring dark text (`#050508`). They float upwards slightly (`transform: translateY(-2px)`) on hover and emit a glowing cyan shadow (`rgba(0, 217, 255, 0.5)`).
  - *Secondary Call-to-Action:* Transparent background with a thin border (`1px solid rgba(255, 255, 255, 0.1)`) and white text. Hovering fills the background slightly with transparent white (`rgba(255, 255, 255, 0.1)`).
  - *Toggle Buttons:* Small rectangular toggle pills (`border-radius: 4px`) with a default transparent grey style. Clicking "ON" transitions the button to a glowing Emerald Green background (`#0e7a0d`), while clicking "OFF" changes it to a glowing Ruby Red background (`#a80000`).
* **Cards/Containers:**
  - *Glassmorphic Panels:* Designed with `.glass-card` classes using a radial gradient from dark grey to black. They feature a thin border (`1px solid rgba(255, 255, 255, 0.06)`), a high blur backdrop-filter (`16px`), and rounded corners (`border-radius: 12px`).
  - *3D Mockup Containers:* Feature isometric skew animations (`perspective(1000px) rotateY(-8deg) rotateX(4deg)`) and smooth floating effects that bounce vertically. On hover, their border brightens to Cyan and emits a soft cyan glow.
* **Inputs/Forms:**
  - *Search inputs:* Feature a clean dark background, rounded corners (`border-radius: 8px` or pill-shaped), and an embedded magnifying glass icon. Active search bars highlight their border and text dynamically.

## 5. Layout Principles
- **Grid Structure:** The website is structured around modular, grid-based layouts. The hero section divides the screen into a `1.2fr` content column and a `0.8fr` visual column.
- **Modular Dashboard:** The interactive preview uses a split layout: a fixed left-side sidebar (`240px`) containing navigation buttons, and a flexible main panel containing stats gauges, profile selectors, and list items.
- **Spacing & Whitespace:** Generous spacing is used to maintain a premium feel. Large section components are separated by deep margins (`padding: 80px 8%`), allowing the dark background and neon glows to breathe.
- **Fixed Navigation:** A sticky navbar (`position: sticky`) remains at the top of the viewport, with a blurred backdrop (`12px`) to overlay content smoothly as the user scrolls.
