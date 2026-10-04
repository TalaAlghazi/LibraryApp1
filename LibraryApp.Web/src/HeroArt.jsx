// Decorative illustration beside the heading on the books page.
function HeroArt() {
  return (
    <div className="hero-art" aria-hidden="true">
      <svg className="hero-illustration" viewBox="0 0 460 280" focusable="false">
        <ellipse cx="232" cy="264" rx="214" ry="10" fill="#173F32" opacity=".07" />

        {/* Stacked books */}
        <rect x="200" y="198" width="200" height="34" rx="4" fill="#1F4A3B" />
        <rect x="372" y="202" width="26" height="26" rx="2" fill="#F3EAD3" />
        <path d="M374 208h22M374 215h22M374 222h22" stroke="#D6C9A8" strokeWidth="1" />
        <rect x="212" y="202" width="2" height="26" fill="#C9A96A" opacity=".8" />
        <rect x="218" y="202" width="2" height="26" fill="#C9A96A" opacity=".8" />
        <text x="286" y="215" className="hero-spine" textAnchor="middle" dominantBaseline="central">
          Places
        </text>

        <rect x="190" y="164" width="196" height="34" rx="4" fill="#2F5D49" />
        <rect x="358" y="168" width="26" height="26" rx="2" fill="#F3EAD3" />
        <path d="M360 174h22M360 181h22M360 188h22" stroke="#D6C9A8" strokeWidth="1" />
        <rect x="202" y="168" width="2" height="26" fill="#C9A96A" opacity=".8" />
        <rect x="208" y="168" width="2" height="26" fill="#C9A96A" opacity=".8" />
        <text x="274" y="181" className="hero-spine" textAnchor="middle" dominantBaseline="central">
          People
        </text>

        <rect x="206" y="130" width="188" height="34" rx="4" fill="#173F32" />
        <rect x="366" y="134" width="26" height="26" rx="2" fill="#F3EAD3" />
        <path d="M368 140h22M368 147h22M368 154h22" stroke="#D6C9A8" strokeWidth="1" />
        <rect x="218" y="134" width="2" height="26" fill="#C9A96A" opacity=".8" />
        <rect x="224" y="134" width="2" height="26" fill="#C9A96A" opacity=".8" />
        <text x="286" y="147" className="hero-spine" textAnchor="middle" dominantBaseline="central">
          Ideas
        </text>

        <g transform="rotate(-3 288 114)">
          <rect x="198" y="98" width="180" height="32" rx="4" fill="#3B6A54" />
          <rect x="350" y="102" width="26" height="24" rx="2" fill="#F3EAD3" />
          <path d="M352 108h22M352 114h22M352 120h22" stroke="#D6C9A8" strokeWidth="1" />
          <rect x="210" y="102" width="2" height="24" fill="#C9A96A" opacity=".8" />
          <rect x="216" y="102" width="2" height="24" fill="#C9A96A" opacity=".8" />
          <text x="274" y="114" className="hero-spine" textAnchor="middle" dominantBaseline="central">
            Books
          </text>
        </g>

        {/* Vase with sprigs */}
        <path
          d="M425 190 C420 160 408 140 396 118 M425 190 C428 158 438 136 450 116 M425 190 C424 160 427 132 424 100"
          fill="none"
          stroke="#5E7F5A"
          strokeWidth="1.6"
          strokeLinecap="round"
        />
        <g fill="#6F8F66">
          <ellipse cx="414" cy="162" rx="7" ry="2.8" transform="rotate(-55 414 162)" />
          <ellipse cx="404" cy="140" rx="7" ry="2.8" transform="rotate(-45 404 140)" />
          <ellipse cx="433" cy="160" rx="7" ry="2.8" transform="rotate(55 433 160)" />
          <ellipse cx="443" cy="138" rx="7" ry="2.8" transform="rotate(45 443 138)" />
          <ellipse cx="419" cy="148" rx="6.5" ry="2.6" transform="rotate(-75 419 148)" />
          <ellipse cx="429" cy="126" rx="6.5" ry="2.6" transform="rotate(75 429 126)" />
        </g>
        <g fill="#F7F1E2" stroke="#C9B98F" strokeWidth="1">
          <circle cx="396" cy="116" r="4.5" />
          <circle cx="450" cy="114" r="4.5" />
          <circle cx="424" cy="98" r="5" />
        </g>
        <g fill="#C9A96A">
          <circle cx="396" cy="116" r="1.5" />
          <circle cx="450" cy="114" r="1.5" />
          <circle cx="424" cy="98" r="1.6" />
        </g>
        <path
          d="M414 232 Q410 208 418 198 L418 190 L432 190 L432 198 Q440 208 436 232 Z"
          fill="#ECE5D3"
          stroke="#BFB292"
          strokeWidth="1"
        />

        {/* Open book */}
        <path d="M8 254 Q70 238 130 250 Q190 240 256 258 L254 264 Q190 247 130 257 Q70 245 8 261 Z" fill="#173F32" />
        <path d="M14 250 Q70 234 130 246 L134 212 Q74 198 24 212 Z" fill="#FBF6EA" stroke="#CDBF9E" strokeWidth="1" />
        <path d="M130 246 Q190 236 250 254 L244 216 Q190 202 134 212 Z" fill="#F6EFDF" stroke="#CDBF9E" strokeWidth="1" />
        <path
          d="M36 220 Q82 212 124 220 M34 228 Q80 220 124 228 M32 236 Q78 228 124 236 M140 220 Q190 214 236 226 M140 228 Q190 222 238 234 M140 236 Q188 230 240 242"
          fill="none"
          stroke="#D9CDB0"
          strokeWidth="1.2"
          strokeLinecap="round"
        />
      </svg>
      <p className="hero-quote">
        Good books,
        <br />
        brighter days.
      </p>
    </div>
  )
}

export default HeroArt
