# Marina AI Style — Design System Reference
> AI-powered wardrobe assistant · OKLCH color space · Auto-generated from CSS tokens

**Theme:** dark (default) with light mode via `data-theme="light"` and `prefers-color-scheme`

Marina AI Style is a modern dark-first design system built on OKLCH color space. Brand accent is pink-red (hue 15°, `oklch(0.59 0.19 15)`) with purple secondary (hue 290°, `oklch(0.55 0.22 290)`). Typography combines Plus Jakarta Sans (display), Inter (body), and Fira Code (mono) — all served locally via `@font-face`. Components use a 4px spacing base, 8–16px radii, and layered shadows in OKLCH black.

---

## Tokens — Colors

All colors use OKLCH for perceptual uniformity. Format: `oklch(L C H)` or `oklch(L C H / alpha)`.

### Brand (Hue 15°)

> Primary brand color — CTAs, active states, links, focus rings

| Name | OKLCH | Token |
|------|-------|-------|
| brand-50 | `oklch(0.97 0.02 15)` | `--color-brand-50` |
| brand-100 | `oklch(0.93 0.035 15)` | `--color-brand-100` |
| brand-200 | `oklch(0.87 0.06 15)` | `--color-brand-200` |
| brand-300 | `oklch(0.78 0.1 15)` | `--color-brand-300` |
| brand-400 | `oklch(0.68 0.15 15)` | `--color-brand-400` |
| brand-500 | `oklch(0.59 0.19 15)` | `--color-brand-500` |
| brand-600 | `oklch(0.5 0.17 15)` | `--color-brand-600` |
| brand-700 | `oklch(0.43 0.14 15)` | `--color-brand-700` |
| brand-800 | `oklch(0.37 0.12 15)` | `--color-brand-800` |
| brand-900 | `oklch(0.32 0.1 15)` | `--color-brand-900` |
| brand-950 | `oklch(0.18 0.06 15)` | `--color-brand-950` |

### Accent (Hue 290°)

> Secondary accent — decorative highlights, visited links

| Name | OKLCH | Token |
|------|-------|-------|
| accent-50 | `oklch(0.97 0.03 290)` | `--color-accent-50` |
| accent-100 | `oklch(0.93 0.05 290)` | `--color-accent-100` |
| accent-200 | `oklch(0.87 0.09 290)` | `--color-accent-200` |
| accent-300 | `oklch(0.78 0.13 290)` | `--color-accent-300` |
| accent-400 | `oklch(0.68 0.18 290)` | `--color-accent-400` |
| accent-500 | `oklch(0.55 0.22 290)` | `--color-accent-500` |
| accent-600 | `oklch(0.47 0.2 290)` | `--color-accent-600` |
| accent-700 | `oklch(0.32 0.12 290)` | `--color-accent-700` |
| accent-800 | `oklch(0.27 0.1 290)` | `--color-accent-800` |
| accent-900 | `oklch(0.23 0.08 290)` | `--color-accent-900` |
| accent-950 | `oklch(0.14 0.05 290)` | `--color-accent-950` |

### Neutral (Hue 0°)

> Backgrounds, text, borders — the structural palette

| Name | OKLCH | Token |
|------|-------|-------|
| neutral-50 | `oklch(0.98 0.005 0)` | `--color-neutral-50` |
| neutral-100 | `oklch(0.96 0.005 0)` | `--color-neutral-100` |
| neutral-200 | `oklch(0.92 0.005 0)` | `--color-neutral-200` |
| neutral-300 | `oklch(0.87 0.005 0)` | `--color-neutral-300` |
| neutral-400 | `oklch(0.71 0.01 0)` | `--color-neutral-400` |
| neutral-500 | `oklch(0.55 0.01 0)` | `--color-neutral-500` |
| neutral-600 | `oklch(0.43 0.01 0)` | `--color-neutral-600` |
| neutral-700 | `oklch(0.32 0.01 0)` | `--color-neutral-700` |
| neutral-800 | `oklch(0.22 0.005 0)` | `--color-neutral-800` |
| neutral-900 | `oklch(0.14 0.005 0)` | `--color-neutral-900` |
| neutral-950 | `oklch(0.07 0.005 0)` | `--color-neutral-950` |

### Status

> Success, warning, error, info — used for feedback states

| Name | OKLCH | Token |
|------|-------|-------|
| success | `oklch(0.72 0.19 145)` | `--color-success` |
| warning | `oklch(0.82 0.17 70)` | `--color-warning` |
| error | `oklch(0.63 0.22 25)` | `--color-error` |
| info | `oklch(0.65 0.15 240)` | `--color-info` |
| purple | `oklch(0.55 0.18 300)` | `--color-purple` |
| teal | `oklch(0.72 0.14 165)` | `--color-teal` |

### Semantic Tokens

High-level tokens that map to primitives. Use these in components — never hardcode primitives.

**Backgrounds**

| Token | Value |
|-------|-------|
| `--bg-app` | `oklch(0.07 0.005 0)` |
| `--bg-page` | `oklch(0.07 0.005 0)` |
| `--bg-surface` | `oklch(0.14 0.005 0)` |
| `--bg-elevated` | `oklch(0.22 0.005 0)` |
| `--bg-overlay` | `oklch(0 0 0 / 0.6)` |
| `--bg-scrim` | `oklch(0 0 0 / 0.4)` |

**Text**

| Token | Value |
|-------|-------|
| `--text-primary` | `oklch(0.98 0.005 0)` |
| `--text-secondary` | `oklch(0.71 0.01 0)` |
| `--text-muted` | `oklch(0.55 0.01 0)` |
| `--text-disabled` | `oklch(0.32 0.01 0)` |
| `--text-on-brand` | `oklch(1 0 0)` |
| `--text-on-accent` | `oklch(1 0 0)` |
| `--text-on-success` | `oklch(1 0 0)` |
| `--text-on-error` | `oklch(1 0 0)` |
| `--text-on-warning` | `oklch(0.15 0 0)` |

**Borders**

| Token | Value |
|-------|-------|
| `--border-default` | `oklch(0.32 0.01 0)` |
| `--border-subtle` | `oklch(0.22 0.005 0)` |
| `--border-strong` | `oklch(0.43 0.01 0)` |
| `--border-focus` | `oklch(0.59 0.19 15)` |
| `--border-error` | `oklch(0.63 0.22 25)` |
| `--border-success` | `oklch(0.72 0.19 145)` |

**Actions**

| Token | Value |
|-------|-------|
| `--action-primary` | `oklch(0.59 0.19 15)` |
| `--action-primary-hover` | `oklch(0.5 0.17 15)` |
| `--action-primary-active` | `oklch(0.43 0.14 15)` |
| `--action-primary-disabled` | `oklch(0.32 0.01 0)` |
| `--action-secondary-bg` | `transparent` |
| `--action-secondary-border` | `oklch(0.43 0.01 0)` |
| `--action-secondary-text` | `oklch(0.87 0.005 0)` |
| `--action-secondary-hover-border` | `oklch(0.59 0.19 15)` |
| `--action-secondary-hover-text` | `oklch(0.59 0.19 15)` |
| `--action-ghost-bg` | `oklch(0.59 0.19 15 / 0.1)` |
| `--action-ghost-text` | `oklch(0.59 0.19 15)` |
| `--action-ghost-hover-bg` | `oklch(0.59 0.19 15 / 0.2)` |

### Light Theme Overrides

These tokens change value when `html[data-theme="light"]` is set:

| Token | Dark Value | Light Value |
|-------|------------|-------------|
| `--bg-app` | `oklch(0.07 0.005 0)` | `oklch(0.98 0.005 0)` |
| `--bg-page` | `oklch(0.07 0.005 0)` | `oklch(0.98 0.005 0)` |
| `--bg-surface` | `oklch(0.14 0.005 0)` | `oklch(1 0 0)` |
| `--bg-elevated` | `oklch(0.22 0.005 0)` | `oklch(0.96 0.005 0)` |
| `--bg-overlay` | `oklch(0 0 0 / 0.6)` | `oklch(0 0 0 / 0.5)` |
| `--bg-scrim` | `oklch(0 0 0 / 0.4)` | `oklch(0 0 0 / 0.3)` |
| `--text-primary` | `oklch(0.98 0.005 0)` | `oklch(0.07 0.005 0)` |
| `--text-secondary` | `oklch(0.71 0.01 0)` | `oklch(0.43 0.01 0)` |
| `--text-disabled` | `oklch(0.32 0.01 0)` | `oklch(0.87 0.005 0)` |
| `--border-default` | `oklch(0.32 0.01 0)` | `oklch(0.92 0.005 0)` |
| `--border-subtle` | `oklch(0.22 0.005 0)` | `oklch(0.96 0.005 0)` |
| `--border-strong` | `oklch(0.43 0.01 0)` | `oklch(0.87 0.005 0)` |
| `--action-secondary-border` | `oklch(0.43 0.01 0)` | `oklch(0.87 0.005 0)` |
| `--action-secondary-text` | `oklch(0.87 0.005 0)` | `oklch(0.32 0.01 0)` |
| `--action-secondary-hover-border` | `oklch(0.59 0.19 15)` | `oklch(0.5 0.17 15)` |
| `--action-secondary-hover-text` | `oklch(0.59 0.19 15)` | `oklch(0.5 0.17 15)` |
| `--surface-nav-active` | `oklch(0.59 0.19 15 / 0.12)` | `oklch(0.59 0.19 15 / 0.08)` |
| `--surface-topbar` | `oklch(0.07 0.005 0 / 0.8)` | `oklch(1 0 0 / 0.8)` |
| `--surface-cta` | `oklch(1 0 0 / 0.2)` | `oklch(0 0 0 / 0.1)` |
| `--surface-cta-hover` | `oklch(1 0 0 / 0.3)` | `oklch(0 0 0 / 0.15)` |
| `--link-default` | `oklch(0.59 0.19 15)` | `oklch(0.5 0.17 15)` |
| `--link-hover` | `oklch(0.68 0.15 15)` | `oklch(0.43 0.14 15)` |
| `--sidebar-nav-bg-active` | `oklch(0.59 0.19 15 / 0.12)` | `oklch(0.59 0.19 15 / 0.08)` |
| `--topbar-bg` | `oklch(0.07 0.005 0 / 0.8)` | `oklch(1 0 0 / 0.8)` |
| `--button-cta-bg` | `oklch(1 0 0 / 0.2)` | `oklch(0 0 0 / 0.1)` |
| `--button-cta-hover-bg` | `oklch(1 0 0 / 0.3)` | `oklch(0 0 0 / 0.15)` |
| `--toggle-bg` | `oklch(0.43 0.01 0)` | `oklch(0.87 0.005 0)` |
| `--success-text` | `oklch(0.72 0.19 145)` | `oklch(0.40 0.15 145)` |
| `--warning-text` | `oklch(0.82 0.17 70)` | `oklch(0.50 0.15 70)` |
| `--error-text` | `oklch(0.63 0.22 25)` | `oklch(0.45 0.18 25)` |
| `--info-text` | `oklch(0.65 0.15 240)` | `oklch(0.45 0.12 240)` |
| `--purple-text` | `oklch(0.55 0.18 300)` | `oklch(0.45 0.15 300)` |
| `--table-row-hover-bg` | `oklch(1 0 0 / 0.03)` | `oklch(0 0 0 / 0.03)` |

### Gradients

| Token | Value |
|-------|-------|
| `--gradient-brand` | `linear-gradient( 135deg, oklch(0.59 0.19 15), oklch(0.5 0.17 15) )` |
| `--gradient-accent` | `linear-gradient( 135deg, oklch(0.32 0.12 290), oklch(0.47 0.2 290) )` |
| `--gradient-brand-accent` | `linear-gradient( 135deg, oklch(0.59 0.19 15), oklch(0.32 0.12 290) )` |

## Tokens — Typography

### Font Families

| Token | Value | Role |
|-------|-------|------|
| `--font-body` | `"Inter", sans-serif` | Body text, UI labels, buttons |
| `--font-display` | `"Plus Jakarta Sans", sans-serif` | Headings, display text, section titles |
| `--font-mono` | `"Fira Code", monospace` | Code snippets, technical labels, data values |

### Font Weights

All three fonts support: 300 (Light), 400 (Regular), 500 (Medium), 600 (SemiBold), 700 (Bold), 800 (ExtraBold), 900 (Black).

### Type Scale

| Role | Size | Weight | Line Height | Usage |
|------|------|--------|-------------|-------|
| display | 48px | 900 | 1.0 | Hero headlines |
| h1 | 36px | 700 | 1.2 | Page titles |
| h2 | 30px | 700 | 1.2 | Section titles |
| h3 | 24px | 600 | 1.3 | Subsections |
| h4 | 20px | 600 | 1.3 | Card titles |
| body | 14px | 400 | 1.6 | Main text |
| caption | 12px | 400 | 1.4 | Labels, metadata |
| overline | 10px | 600 | 1.25 | Eyebrows, tags |

## Tokens — Spacing

Base unit: 4px. All spacing is a multiple of 4.

### Scale

| Token | Value |
|-------|-------|
| `--space-0` | 0 |
| `--space-1` | 4px |
| `--space-2` | 8px |
| `--space-3` | 12px |
| `--space-4` | 16px |
| `--space-5` | 20px |
| `--space-6` | 24px |
| `--space-8` | 32px |
| `--space-10` | 40px |
| `--space-12` | 48px |
| `--space-16` | 64px |

### Component Spacing

| Token | Value | Component |
|-------|-------|-----------|
| `--space-badge-px` | 12px | badge px |
| `--space-badge-py` | 4px | badge py |
| `--space-alert-px` | 20px | alert px |
| `--space-alert-py` | 14px | alert py |
| `--space-alert-gap` | 8px | alert gap |
| `--space-input-px` | 16px | input px |
| `--space-input-py` | 12px | input py |
| `--space-modal-padding` | 2rem | modal padding |
| `--space-card-padding` | 1.25rem | card padding |
| `--space-sidebar-padding` | 2rem 0 | sidebar padding |
| `--space-swatch-info-px` | 8px | swatch info px |
| `--space-swatch-info-py` | 6px | swatch info py |
| `--space-icon-cell-padding` | 12px 8px | icon cell padding |
| `--space-toast-px` | 20px | toast px |
| `--space-toast-py` | 14px | toast py |
| `--space-toast-gap` | 10px | toast gap |
| `--space-btn-sm-px` | 14px | btn sm px |
| `--space-btn-sm-py` | 6px | btn sm py |
| `--space-btn-lg-px` | 28px | btn lg px |
| `--space-btn-lg-py` | 14px | btn lg py |

## Tokens — Border Radius

| Token | Value | Typical Usage |
|-------|-------|---------------|
| `--radius-none` | 0 | Sharp corners |
| `--radius-sm` | 4px | Tags, small chips, progress bars |
| `--radius-md` | 8px | Buttons (sm), inputs |
| `--radius-lg` | 12px | Buttons (default), cards, alerts, badges |
| `--radius-xl` | 16px | Modals, large containers, sidebar CTA |
| `--radius-2xl` | 20px | Hero sections |
| `--radius-full` | 9999px | Pills, avatars, toggles |

## Tokens — Shadows

All shadows use OKLCH black (`oklch(0 0 0)`) with increasing opacity.

| Token | Value |
|-------|-------|
| `--shadow-xs` | `0 1px 2px oklch(0 0 0 / 0.15)` |
| `--shadow-sm` | `0 2px 4px oklch(0 0 0 / 0.2)` |
| `--shadow-md` | `0 4px 12px oklch(0 0 0 / 0.25)` |
| `--shadow-lg` | `0 8px 25px -5px oklch(0 0 0 / 0.3)` |
| `--shadow-xl` | `0 12px 40px -8px oklch(0 0 0 / 0.35)` |
| `--shadow-2xl` | `0 20px 60px -12px oklch(0 0 0 / 0.4)` |
| `--elevation-0` | `none` |
| `--elevation-1` | `0 1px 2px oklch(0 0 0 / 0.15)` |
| `--elevation-2` | `0 2px 4px oklch(0 0 0 / 0.2)` |
| `--elevation-3` | `0 4px 12px oklch(0 0 0 / 0.25)` |
| `--elevation-4` | `0 8px 25px -5px oklch(0 0 0 / 0.3)` |
| `--elevation-5` | `0 12px 40px -8px oklch(0 0 0 / 0.35)` |

## Tokens — Motion

### Duration

| Token | Value |
|-------|-------|
| `--duration-fast` | `0.15s` |
| `--duration-normal` | `0.2s` |
| `--duration-slow` | `0.3s` |
| `--duration-slower` | `0.4s` |

### Easing

| Token | Value |
|-------|-------|
| `--ease-default` | `ease` |
| `--ease-in` | `ease-in` |
| `--ease-out` | `ease-out` |
| `--ease-in-out` | `ease-in-out` |
| `--ease-bounce` | `cubic-bezier(0.34, 1.56, 0.64, 1)` |

### Backdrop Blur

| Token | Value |
|-------|-------|
| `--backdrop-blur-sm` | `blur(4px)` |
| `--backdrop-blur-md` | `blur(16px)` |
| `--backdrop-blur-lg` | `blur(24px)` |

### Transition Composites

| Token | Value |
|-------|-------|
| `--transition-colors` | `color 0.2s ease, background-color 0.2s ease, border-color 0.2s ease` |
| `--transition-transform` | `transform 0.2s ease` |
| `--transition-shadow` | `box-shadow 0.2s ease` |
| `--transition-all` | `all 0.2s ease` |

## Tokens — Sizes

| Token | Value | Component |
|-------|-------|-----------|
| `--size-toggle-w` | 44px | toggle w |
| `--size-toggle-h` | 24px | toggle h |
| `--size-toggle-knob` | 20px | toggle knob |
| `--size-toggle-offset` | 2px | toggle offset |
| `--size-progress-h` | 6px | progress h |
| `--size-modal-max-w` | 480px | modal max w |
| `--size-modal-width` | 90% | modal width |
| `--size-modal-padding` | 2rem | modal padding |
| `--size-sidebar-logo` | 40px | sidebar logo |
| `--size-sidebar-width` | 272px | sidebar width |
| `--size-section-icon` | 32px | section icon |
| `--size-swatch-color-h` | 64px | swatch color h |
| `--font-size-section-icon` | 0.9rem | font section icon |
| `--font-size-icon-cell` | 1.5rem | font icon cell |
| `--font-size-toast` | 14px | font toast |

## Tokens — Z-Index Layers

| Token | Value | Purpose |
|-------|-------|---------|
| `--z-base` | 0 | Default stacking |
| `--z-dropdown` | 10 | Dropdowns, popovers |
| `--z-sticky` | 20 | Sticky headers, topbar |
| `--z-sidebar` | 30 | Sidebar navigation |
| `--z-overlay` | 100 | Modal overlays, backdrops |
| `--z-modal` | 200 | Modal content |
| `--z-toast` | 300 | Toast notifications |

## Tokens — Surface States

| Token | Value |
|-------|-------|
| `--surface-nav-active` | `oklch(0.59 0.19 15 / 0.12)` |
| `--surface-topbar` | `oklch(0.07 0.005 0 / 0.8)` |
| `--surface-cta` | `oklch(1 0 0 / 0.2)` |
| `--surface-cta-hover` | `oklch(1 0 0 / 0.3)` |

## Tokens — Links

| Token | Value |
|-------|-------|
| `--link-default` | `oklch(0.59 0.19 15)` |
| `--link-hover` | `oklch(0.68 0.15 15)` |
| `--link-visited` | `oklch(0.47 0.2 290)` |
| `--link-disabled` | `oklch(0.32 0.01 0)` |

## Tokens — Chart Colors

| Token | Value |
|-------|-------|
| `--chart-1` | `oklch(0.59 0.19 15)` |
| `--chart-2` | `oklch(0.32 0.12 290)` |
| `--chart-3` | `oklch(0.65 0.15 240)` |
| `--chart-4` | `oklch(0.72 0.19 145)` |
| `--chart-5` | `oklch(0.82 0.17 70)` |
| `--chart-6` | `oklch(0.55 0.18 300)` |
| `--chart-7` | `oklch(0.63 0.22 25)` |
| `--chart-8` | `oklch(0.72 0.14 165)` |
| `--chart-1-light` | `oklch(0.59 0.19 15 / 0.15)` |
| `--chart-2-light` | `oklch(0.32 0.12 290 / 0.15)` |
| `--chart-3-light` | `oklch(0.65 0.15 240 / 0.15)` |
| `--chart-4-light` | `oklch(0.72 0.19 145 / 0.15)` |
| `--chart-5-light` | `oklch(0.82 0.17 70 / 0.15)` |
| `--chart-6-light` | `oklch(0.55 0.18 300 / 0.15)` |
| `--chart-7-light` | `oklch(0.63 0.22 25 / 0.15)` |
| `--chart-8-light` | `oklch(0.72 0.14 165 / 0.15)` |

## Tokens — Overlays & Modals

| Token | Value |
|-------|-------|
| `--overlay-rgb` | `0, 0, 0` |
| `--modal-content-radius` | `16px` |
| `--modal-bg` | `oklch(0 0 0 / 0.6)` |
| `--modal-content-bg` | `oklch(0.14 0.005 0)` |
| `--modal-content-border` | `oklch(0.32 0.01 0)` |

## Tokens — RGB Bases

| Token | Value |
|-------|-------|
| `--scrim-rgb` | `0, 0, 0` |

## Tokens — Focus Ring

| Token | Value |
|-------|-------|
| `--focus-ring` | `0 0 0 2px oklch(0.07 0.005 0), 0 0 0 4px oklch(0.59 0.19 15)` |

## Component Tokens

Component-specific tokens extracted from component CSS files. Use these to build consistent UI elements.

| Token | Value |
|-------|-------|
| `--button-cta-bg` | `oklch(1 0 0 / 0.2)` |
| `--button-cta-hover-bg` | `oklch(1 0 0 / 0.3)` |
| `--button-radius` | `12px` |
| `--button-radius-sm` | `8px` |
| `--button-radius-lg` | `16px` |
| `--button-primary-bg` | `linear-gradient( 135deg, oklch(0.59 0.19 15), oklch(0.5 0.17 15) )` |
| `--button-primary-text` | `oklch(1 0 0)` |
| `--button-primary-border` | `transparent` |
| `--button-primary-active` | `oklch(0.43 0.14 15)` |
| `--button-primary-disabled-bg` | `oklch(0.32 0.01 0)` |
| `--button-primary-disabled-text` | `oklch(0.32 0.01 0)` |
| `--button-primary-hover-shadow` | `0 4px 12px oklch(0 0 0 / 0.25)` |
| `--button-secondary-bg` | `transparent` |
| `--button-secondary-text` | `oklch(0.87 0.005 0)` |
| `--button-secondary-border` | `oklch(0.43 0.01 0)` |
| `--button-secondary-hover-border` | `oklch(0.59 0.19 15)` |
| `--button-secondary-hover-text` | `oklch(0.59 0.19 15)` |
| `--button-ghost-bg` | `oklch(0.59 0.19 15 / 0.1)` |
| `--button-ghost-text` | `oklch(0.59 0.19 15)` |
| `--button-ghost-hover-bg` | `oklch(0.59 0.19 15 / 0.2)` |
| `--button-danger-bg` | `oklch(0.63 0.22 25 / 0.1)` |
| `--button-danger-text` | `oklch(0.63 0.22 25)` |
| `--button-danger-border` | `oklch(0.63 0.22 25 / 0.3)` |
| `--button-success-bg` | `oklch(0.72 0.19 145 / 0.1)` |
| `--button-success-text` | `oklch(0.72 0.19 145)` |
| `--button-success-border` | `oklch(0.72 0.19 145 / 0.3)` |
| `--input-radius` | `12px` |
| `--input-bg` | `oklch(0.22 0.005 0)` |
| `--input-text` | `oklch(0.98 0.005 0)` |
| `--input-placeholder` | `oklch(0.55 0.01 0)` |
| `--input-border` | `oklch(0.43 0.01 0)` |
| `--input-border-focus` | `oklch(0.59 0.19 15)` |
| `--input-border-error` | `oklch(0.63 0.22 25)` |
| `--toggle-bg` | `oklch(0.43 0.01 0)` |
| `--toggle-knob` | `oklch(1 0 0)` |
| `--badge-radius` | `9999px` |
| `--alert-radius` | `12px` |
| `--toast-radius` | `12px` |
| `--success-bg` | `oklch(0.72 0.19 145 / 0.1)` |
| `--success-text` | `oklch(0.72 0.19 145)` |
| `--success-border` | `oklch(0.72 0.19 145 / 0.3)` |
| `--warning-bg` | `oklch(0.82 0.17 70 / 0.1)` |
| `--warning-text` | `oklch(0.82 0.17 70)` |
| `--warning-border` | `oklch(0.82 0.17 70 / 0.3)` |
| `--error-bg` | `oklch(0.63 0.22 25 / 0.1)` |
| `--error-text` | `oklch(0.63 0.22 25)` |
| `--error-border` | `oklch(0.63 0.22 25 / 0.3)` |
| `--info-bg` | `oklch(0.65 0.15 240 / 0.1)` |
| `--info-text` | `oklch(0.65 0.15 240)` |
| `--info-border` | `oklch(0.65 0.15 240 / 0.3)` |
| `--purple-bg` | `oklch(0.55 0.18 300 / 0.1)` |
| `--purple-text` | `oklch(0.55 0.18 300)` |
| `--purple-border` | `oklch(0.55 0.18 300 / 0.3)` |
| `--brand-bg` | `oklch(0.59 0.19 15 / 0.1)` |
| `--brand-text` | `oklch(0.59 0.19 15)` |
| `--brand-border` | `oklch(0.59 0.19 15 / 0.3)` |
| `--toast-bg` | `oklch(0.14 0.005 0)` |
| `--toast-text` | `oklch(0.98 0.005 0)` |
| `--toast-shadow` | `0 12px 40px -8px oklch(0 0 0 / 0.35)` |
| `--card-radius` | `16px` |
| `--card-bg` | `oklch(0.14 0.005 0)` |
| `--card-border` | `oklch(0.32 0.01 0)` |
| `--card-border-hover` | `oklch(0.43 0.01 0)` |
| `--card-shadow` | `0 2px 4px oklch(0 0 0 / 0.2)` |
| `--card-shadow-hover` | `0 8px 25px -5px oklch(0 0 0 / 0.3)` |
| `--table-header-bg` | `oklch(0.14 0.005 0)` |
| `--table-row-hover-bg` | `oklch(1 0 0 / 0.03)` |
| `--table-border` | `oklch(0.22 0.005 0)` |
| `--table-border-header` | `oklch(0.32 0.01 0)` |

## Accessibility

### Focus Ring

`--focus-ring: 0 0 0 2px oklch(0.07 0.005 0), 0 0 0 4px oklch(0.59 0.19 15)`

Applied to interactive elements on `:focus`. Creates a 2px app-background gap + 4px brand-colored ring.

### Reduced Motion

When `prefers-reduced-motion: reduce` is set:
- All animations and transitions are disabled (duration → 0.01ms)
- `scroll-behavior` set to `auto`

## Do's and Don'ts

### Do
- Use OKLCH color space for all colors
- Reference semantic tokens (`--bg-app`, `--text-primary`) instead of primitives
- Use Plus Jakarta Sans for headings, Inter for body, Fira Code for code
- Keep spacing on the 4px grid
- Use 8px radius for buttons/inputs, 12px for cards, 16px for modals
- Apply shadows from the elevation scale (`--shadow-xs` → `--shadow-2xl`)
- Support both dark and light themes via `data-theme` attribute
- Respect `prefers-reduced-motion` for animations
- Use `--focus-ring` for keyboard navigation indicators

### Don't
- Do not hardcode hex colors — use OKLCH tokens
- Do not mix font families within a component
- Do not use spacing values not divisible by 4
- Do not use border radius above 20px (except `--radius-full`)
- Do not apply shadows to flat elements (badges, tags)
- Do not bypass semantic tokens to use primitives directly
- Do not use status colors (success/error) for decorative purposes

## Surfaces

| Level | Token | Value | Purpose |
|-------|-------|-------|---------|
| 0 | `--bg-app` | `oklch(0.07 0.005 0)` | Page-level background (level 0) |
| 1 | `--bg-surface` | `oklch(0.14 0.005 0)` | Card and container backgrounds (level 1) |
| 2 | `--bg-elevated` | `oklch(0.22 0.005 0)` | Interactive elements, inputs (level 2) |
| 3 | `--bg-overlay` | `oklch(0 0 0 / 0.6)` | Modal and dropdown overlays (level 3) |

## Layout

- **Sidebar width:** 272px
- **Sidebar logo:** 40px
- **Section icon:** 32px
- **Modal max-width:** 480px
- **Modal width:** 90%
- **Toggle size:** 44px × 24px
- **Progress height:** 6px
- **Section gap:** 64px
- **Card padding:** 1.25rem
- **Content max-width:** 900px (from layout.css)

## Agent Prompt Guide

**Quick Color Reference**
- Primary accent: `oklch(0.59 0.19 15)`
- Secondary accent: `oklch(0.55 0.22 290)`
- Background: `oklch(0.07 0.005 0)`
- Surface: `oklch(0.14 0.005 0)`
- Elevated: `oklch(0.22 0.005 0)`
- Text primary: `oklch(0.98 0.005 0)`
- Text secondary: `oklch(0.71 0.01 0)`
- Border: `oklch(0.32 0.01 0)`
- Focus ring: `0 0 0 2px oklch(0.07 0.005 0), 0 0 0 4px oklch(0.59 0.19 15)`

**Example Component Prompts**

1. **Primary button:** background `linear-gradient( 135deg, oklch(0.59 0.19 15), oklch(0.5 0.17 15) )`, text `oklch(1 0 0)`, Inter 14px/600, 12px radius, 8px 20px padding. Hover: translateY(-1px) + shadow-md.

2. **Card:** background `oklch(0.14 0.005 0)`, border 1px `oklch(0.32 0.01 0)`, 16px radius, 1.25rem padding. Hover: border-color → border-strong, translateY(-2px), shadow-lg.

3. **Text input:** background `oklch(0.22 0.005 0)`, border 1px `oklch(0.43 0.01 0)`, 12px radius, 12px 16px padding. Focus: border → `oklch(0.59 0.19 15)` + focus-ring. Error: border → border-error.

4. **Badge:** pill shape (9999px radius), 4px 12px padding, 12px/600 text. Variants use 10% alpha bg + 30% alpha border of semantic color.

5. **Alert:** 12px radius, 14px 20px padding, flex with 8px icon gap. Background/text/border from semantic status tokens (info/success/warning/error).

## Quick Start

### CSS Custom Properties

```css
:root {
  /* Brand */
  --color-brand-500: oklch(0.59 0.19 15);
  --color-accent-500: oklch(0.55 0.22 290);

  /* Semantic */
  --bg-app: oklch(0.07 0.005 0);
  --bg-surface: oklch(0.14 0.005 0);
  --bg-elevated: oklch(0.22 0.005 0);
  --text-primary: oklch(0.98 0.005 0);
  --text-secondary: oklch(0.71 0.01 0);
  --border-default: oklch(0.32 0.01 0);
  --action-primary: oklch(0.59 0.19 15);

  /* Typography */
  --font-display: "Plus Jakarta Sans", sans-serif;
  --font-body: "Inter", sans-serif;
  --font-mono: "Fira Code", monospace;

  /* Spacing */
  --space-4: 16px;
  --space-6: 24px;

  /* Radius */
  --radius-md: 8px;
  --radius-lg: 12px;
  --radius-xl: 16px;

  /* Shadows */
  --shadow-md: 0 4px 12px oklch(0 0 0 / 0.25);

  /* Motion */
  --duration-normal: 0.2s;
  --ease-default: ease;
}
```