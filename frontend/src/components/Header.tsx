import { useRoomContext } from '../context/RoomContext';
import { useEffect, useState } from 'react';
import { NavLink, Link, useLocation } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';

export default function Header() {
  const { resetRoomFilterData } = useRoomContext();
  const { pathname } = useLocation();
  const [scrolled, setScrolled] = useState(false);
  const [mobileOpen, setMobileOpen] = useState(false);
  const user = useAuthStore((state) => state.user);
  const logout = useAuthStore((state) => state.logout);

  useEffect(() => {
    const handler = () => setScrolled(window.scrollY > 50);
    window.addEventListener('scroll', handler);
    return () => window.removeEventListener('scroll', handler);
  }, []);

  const isLoggedIn = Boolean(user);
  const isGuest = user?.role === 'Guest';
  const isTransparentRoute = pathname === '/' || pathname.startsWith('/room/');
  const headerActive = isTransparentRoute ? scrolled : true;

  const navLinks = [
    { label: 'Home', to: '/' },
    { label: 'Rooms', to: '/rooms' },
    ...(isLoggedIn && isGuest ? [
      { label: 'Favorites', to: '/favorites' },
      { label: 'Book', to: '/checkout' },
    ] : []),
    ...(isLoggedIn && !isGuest ? [
      { label: 'Admin', to: '/admin' },
    ] : []),
  ];

  return (
    <header
      className={`fixed z-50 w-full min-h-[72px] py-6 transition-colors duration-300 ${headerActive ? 'bg-white shadow-lg' : 'bg-transparent'}`}
    >
      <div className="container mx-auto flex h-full items-center gap-y-6 lg:justify-between lg:gap-y-0 max-w-7xl">
        <Link
          to="/"
          onClick={resetRoomFilterData}
          className={`block shrink-0 font-primary text-2xl transition ${headerActive ? 'text-primary' : 'text-white'}`}
          aria-label="GrandStay Hotel home"
        >
          GrandStay Hotel
        </Link>

        <button
          onClick={() => setMobileOpen(!mobileOpen)}
          className={`lg:hidden ${headerActive ? 'text-primary' : 'text-white'} text-2xl`}
          aria-label="Toggle menu"
        >
          &#9776;
        </button>

        <nav className={`${headerActive ? 'text-primary' : 'text-white'} hidden lg:flex flex-wrap items-center gap-x-8 font-tertiary text-[15px] uppercase tracking-[3px]`}>
          {navLinks.map((link) => (
            <NavLink
              key={link.label}
              to={link.to}
              onClick={link.to === '/' ? resetRoomFilterData : undefined}
              className={({ isActive }) => `transition hover:text-accent ${isActive ? 'text-accent' : ''}`}
            >
              {link.label}
            </NavLink>
          ))}
          {isLoggedIn ? (
            <>
              <NavLink
                to="/profile"
                className={({ isActive }) => `transition hover:text-accent ${isActive ? 'text-accent' : ''}`}
              >
                Profile
              </NavLink>
              <button
                onClick={() => {
                  logout();
                  setMobileOpen(false);
                }}
                className="transition hover:text-accent"
              >
                Logout
              </button>
            </>
          ) : (
            <NavLink
              to="/login"
              className={({ isActive }) => `transition hover:text-accent ${isActive ? 'text-accent' : ''}`}
            >
              Login
            </NavLink>
          )}
        </nav>

        {mobileOpen && (
          <nav className={`absolute left-0 top-full w-full ${headerActive ? 'bg-white' : 'bg-primary'} flex flex-col gap-4 px-6 py-6 lg:hidden`}>
            {navLinks.map((link) => (
              <NavLink
                key={link.label}
                to={link.to}
                onClick={() => setMobileOpen(false)}
                className={({ isActive }) => `transition hover:text-accent ${isActive ? 'text-accent' : ''}`}
              >
                {link.label}
              </NavLink>
            ))}
            {isLoggedIn ? (
              <>
                <NavLink to="/profile" onClick={() => setMobileOpen(false)} className="transition hover:text-accent">
                  Profile
                </NavLink>
                <button
                  onClick={() => {
                    logout();
                    setMobileOpen(false);
                  }}
                  className="text-left transition hover:text-accent"
                >
                  Logout
                </button>
              </>
            ) : (
              <NavLink to="/login" onClick={() => setMobileOpen(false)} className="transition hover:text-accent">
                Login
              </NavLink>
            )}
          </nav>
        )}
      </div>
    </header>
  );
}
