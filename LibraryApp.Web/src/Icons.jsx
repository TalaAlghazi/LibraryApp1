// Small inline SVG icons shared across the app.
const line = { fill: 'none', stroke: 'currentColor', strokeWidth: 1.8, strokeLinecap: 'round', strokeLinejoin: 'round' }

export function BookIcon(props) {
  return (
    <svg viewBox="0 0 32 24" aria-hidden="true" focusable="false" {...props}>
      <path {...line} d="M16 5.2C12.6 2.9 8 2.2 2.5 2.8v16.6c5.5-.6 10.1.1 13.5 2.4 3.4-2.3 8-3 13.5-2.4V2.8C24 2.2 19.4 2.9 16 5.2z" />
      <path {...line} d="M16 5.2v16.6" />
    </svg>
  )
}

export function UserIcon(props) {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false" {...props}>
      <path {...line} d="M12 12a4 4 0 1 0 0-8 4 4 0 0 0 0 8zm-7 8a7 7 0 0 1 14 0" />
    </svg>
  )
}

export function MailIcon(props) {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false" {...props}>
      <rect {...line} x="3.5" y="5.5" width="17" height="13" rx="2" />
      <path {...line} d="M4 7l8 6 8-6" />
    </svg>
  )
}

export function LockIcon(props) {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false" {...props}>
      <rect {...line} x="5" y="11" width="14" height="9" rx="2" />
      <path {...line} d="M8 11V8a4 4 0 0 1 8 0v3" />
    </svg>
  )
}

export function EyeIcon({ crossed = false, ...props }) {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false" {...props}>
      <path {...line} strokeWidth={1.7} d="M2.5 12S6 5.5 12 5.5 21.5 12 21.5 12 18 18.5 12 18.5 2.5 12 2.5 12z" />
      <circle {...line} strokeWidth={1.7} cx="12" cy="12" r="3" />
      {crossed && <path {...line} strokeWidth={1.7} d="M4 4l16 16" />}
    </svg>
  )
}

export function SearchIcon(props) {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false" {...props}>
      <circle {...line} strokeWidth={2} cx="11" cy="11" r="7" />
      <path {...line} strokeWidth={2} d="M20 20l-3.5-3.5" />
    </svg>
  )
}

export function MenuIcon(props) {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false" {...props}>
      <path {...line} d="M4 7h16M4 12h16M4 17h16" />
    </svg>
  )
}

// Small leaf ornament printed on the generated book covers.
export function SprigIcon(props) {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false" {...props}>
      <path {...line} strokeWidth={1.2} d="M12 3c-3 4-3 8 0 12 3-4 3-8 0-12z" />
      <path {...line} strokeWidth={1.2} d="M4 18c5 0 7-1 8-3 1 2 3 3 8 3" />
    </svg>
  )
}
