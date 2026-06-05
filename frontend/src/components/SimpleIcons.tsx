import React from 'react';

type IconProps = React.SVGProps<SVGSVGElement> & { size?: number };

export const Wifi = ({ size = 20, ...props }: IconProps) => (
  <svg viewBox="0 0 24 24" width={size} height={size} fill="none" stroke="currentColor" strokeWidth={1.5} strokeLinecap="round" strokeLinejoin="round" {...props}>
    <path d="M2 8.5a15 15 0 0 1 20 0" />
    <path d="M6 12.5a9 9 0 0 1 12 0" />
    <path d="M10 16.5a3 3 0 0 1 4 0" />
    <circle cx="12" cy="19" r="1" fill="currentColor" stroke="none" />
  </svg>
);

export const Coffee = ({ size = 20, ...props }: IconProps) => (
  <svg viewBox="0 0 24 24" width={size} height={size} fill="none" stroke="currentColor" strokeWidth={1.5} strokeLinecap="round" strokeLinejoin="round" {...props}>
    <rect x="3" y="4" width="14" height="12" rx="2" />
    <path d="M7 20h6" />
    <path d="M21 8v2a4 4 0 0 1-4 4" />
  </svg>
);

export const Bath = ({ size = 20, ...props }: IconProps) => (
  <svg viewBox="0 0 24 24" width={size} height={size} fill="none" stroke="currentColor" strokeWidth={1.5} strokeLinecap="round" strokeLinejoin="round" {...props}>
    <path d="M3 11h18v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z" />
    <path d="M7 11V8a3 3 0 0 1 6 0v3" />
  </svg>
);

export const Parking = ({ size = 20, ...props }: IconProps) => (
  <svg viewBox="0 0 24 24" width={size} height={size} fill="none" stroke="currentColor" strokeWidth={1.5} strokeLinecap="round" strokeLinejoin="round" {...props}>
    <path d="M12 3v18" />
    <path d="M7 6h5a3 3 0 0 1 0 6H7z" />
  </svg>
);

export const Pool = ({ size = 20, ...props }: IconProps) => (
  <svg viewBox="0 0 24 24" width={size} height={size} fill="none" stroke="currentColor" strokeWidth={1.5} strokeLinecap="round" strokeLinejoin="round" {...props}>
    <path d="M2 16c4-2 8-2 12 0s8 2 12 0" />
    <path d="M2 12c4-2 8-2 12 0s8 2 12 0" />
  </svg>
);

export const Breakfast = ({ size = 20, ...props }: IconProps) => (
  <svg viewBox="0 0 24 24" width={size} height={size} fill="none" stroke="currentColor" strokeWidth={1.5} strokeLinecap="round" strokeLinejoin="round" {...props}>
    <path d="M3 7h16v6a3 3 0 0 1-3 3H6a3 3 0 0 1-3-3z" />
    <path d="M8 3v4" />
  </svg>
);

export const Gym = ({ size = 20, ...props }: IconProps) => (
  <svg viewBox="0 0 24 24" width={size} height={size} fill="none" stroke="currentColor" strokeWidth={1.5} strokeLinecap="round" strokeLinejoin="round" {...props}>
    <rect x="2" y="10" width="6" height="4" />
    <rect x="16" y="10" width="6" height="4" />
    <path d="M9 12h6" />
  </svg>
);

export const Drinks = ({ size = 20, ...props }: IconProps) => (
  <svg viewBox="0 0 24 24" width={size} height={size} fill="none" stroke="currentColor" strokeWidth={1.5} strokeLinecap="round" strokeLinejoin="round" {...props}>
    <path d="M8 3h8l-1 7a4 4 0 0 1-4 3 4 4 0 0 1-4-3z" />
    <path d="M12 20v-4" />
  </svg>
);

export const ArrowsFullscreen = ({ size = 18, ...props }: IconProps) => (
  <svg viewBox="0 0 24 24" width={size} height={size} fill="none" stroke="currentColor" strokeWidth={1.5} strokeLinecap="round" strokeLinejoin="round" {...props}>
    <path d="M3 9V3h6" />
    <path d="M21 15v6h-6" />
  </svg>
);

export const People = ({ size = 18, ...props }: IconProps) => (
  <svg viewBox="0 0 24 24" width={size} height={size} fill="none" stroke="currentColor" strokeWidth={1.5} strokeLinecap="round" strokeLinejoin="round" {...props}>
    <circle cx="9" cy="8" r="2" />
    <path d="M17 11v2a4 4 0 0 1-4 4H7" />
  </svg>
);

export const ChevronDown = ({ size = 16, ...props }: IconProps) => (
  <svg viewBox="0 0 24 24" width={size} height={size} fill="none" stroke="currentColor" strokeWidth={2} strokeLinecap="round" strokeLinejoin="round" {...props}>
    <path d="M6 9l6 6 6-6" />
  </svg>
);

export const Calendar = ({ size = 18, ...props }: IconProps) => (
  <svg viewBox="0 0 24 24" width={size} height={size} fill="none" stroke="currentColor" strokeWidth={1.5} strokeLinecap="round" strokeLinejoin="round" {...props}>
    <rect x="3" y="4" width="18" height="18" rx="2" />
    <path d="M16 2v4" />
    <path d="M8 2v4" />
  </svg>
);

export const Check = ({ size = 16, ...props }: IconProps) => (
  <svg viewBox="0 0 24 24" width={size} height={size} fill="none" stroke="currentColor" strokeWidth={2} strokeLinecap="round" strokeLinejoin="round" {...props}>
    <path d="M20 6L9 17l-5-5" />
  </svg>
);

export const Bell = ({ size = 18, ...props }: IconProps) => (
  <svg viewBox="0 0 24 24" width={size} height={size} fill="none" stroke="currentColor" strokeWidth={1.5} strokeLinecap="round" strokeLinejoin="round" {...props}>
    <path d="M15 17h5l-1.405-1.405A2.032 2.032 0 0 1 18 14.158V11a6 6 0 0 0-5-5.916V4a1 1 0 0 0-2 0v1.084A6 6 0 0 0 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h11z" />
    <path d="M13.73 21a2 2 0 0 1-3.46 0" />
  </svg>
);

export const Circle = ({ size = 12, ...props }: IconProps) => (
  <svg viewBox="0 0 24 24" width={size} height={size} fill="currentColor" {...props}>
    <circle cx="12" cy="12" r="10" />
  </svg>
);

export const Search = ({ size = 14, ...props }: IconProps) => (
  <svg viewBox="0 0 24 24" width={size} height={size} fill="none" stroke="currentColor" strokeWidth={1.6} strokeLinecap="round" strokeLinejoin="round" {...props}>
    <circle cx="11" cy="11" r="6" />
    <path d="M21 21l-4.35-4.35" />
  </svg>
);

export default {};
