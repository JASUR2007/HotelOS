import { NavLink, useLocation } from 'react-router-dom';
import { sidebarGroups } from '../sidebar/sidebarItems';
import { useUiStore } from '../store/uiStore';
import { useAuthStore } from '../../store/authStore';
import { useWebsocketStatus } from '../hooks/useWebsocketStatus';

function GroupIcon({ icon }: { icon: string }) {
  const icons: Record<string, JSX.Element> = {
    Zap: (
      <svg viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" strokeWidth={1.5} strokeLinecap="round" strokeLinejoin="round">
        <path d="M13 2 L3 14h7l-1 8 10-12h-7z" />
      </svg>
    ),
    Building: (
      <svg viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" strokeWidth={1.5}>
        <rect x="3" y="3" width="18" height="18" rx="2" />
        <path d="M8 7h2M8 11h2M8 15h2M13 7h2M13 11h2M13 15h2" stroke="currentColor" strokeWidth={1} />
      </svg>
    ),
    UtensilsCrossed: (
      <svg viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" strokeWidth={1.5} strokeLinecap="round" strokeLinejoin="round">
        <path d="M7 2l5 10" />
        <path d="M4 8l6 6" />
        <path d="M16 2l-1 6" />
        <path d="M19 8l-6 6" />
      </svg>
    ),
    CreditCard: (
      <svg viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" strokeWidth={1.5} strokeLinecap="round" strokeLinejoin="round">
        <rect x="2" y="5" width="20" height="14" rx="2" />
        <path d="M2 10h20" strokeWidth={1} />
      </svg>
    ),
    Shield: (
      <svg viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" strokeWidth={1.5} strokeLinecap="round" strokeLinejoin="round">
        <path d="M12 2l8 4v6c0 5-4 9-8 11-4-2-8-6-8-11V6l8-4z" />
      </svg>
    ),
    RadioTower: (
      <svg viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" strokeWidth={1.5} strokeLinecap="round" strokeLinejoin="round">
        <path d="M12 2v20" />
        <path d="M8 8a4 4 0 0 1 8 0" />
        <path d="M6 12a8 8 0 0 1 12 0" />
      </svg>
    ),
    PieChart: (
      <svg viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" strokeWidth={1.5} strokeLinecap="round" strokeLinejoin="round">
        <circle cx="12" cy="12" r="9" />
        <path d="M12 3v9l6 2" />
      </svg>
    ),
    Settings: (
      <svg viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" strokeWidth={1.5} strokeLinecap="round" strokeLinejoin="round">
        <circle cx="12" cy="12" r="3" />
        <path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l0 0a2 2 0 1 1-2.83 2.83l0 0A1.65 1.65 0 0 0 15 19.4a1.65 1.65 0 0 0-1.82.33l0 0a2 2 0 1 1-2.83-2.83l0 0A1.65 1.65 0 0 0 8.6 15a1.65 1.65 0 0 0-.33-1.82l0 0a2 2 0 1 1 2.83-2.83l0 0A1.65 1.65 0 0 0 13 8.6a1.65 1.65 0 0 0 1.82-.33l0 0a2 2 0 1 1 2.83 2.83l0 0A1.65 1.65 0 0 0 19.4 9z" />
      </svg>
    ),
  };
  return <span className="mr-3 text-lg text-accent">{icons[icon] ?? <span>•</span>}</span>;
}

export default function Sidebar() {
  const collapsed = useUiStore((s) => s.sidebarCollapsed);
  const toggleSidebar = useUiStore((s) => s.toggleSidebar);
  const groups = useUiStore((s) => s.sidebarGroups);
  const toggleGroup = useUiStore((s) => s.toggleGroup);
  const { connected } = useWebsocketStatus();
  const location = useLocation();
  const userRole = useAuthStore((s) => s.user?.role);

  const isActiveIn = (items: { path: string }[]) =>
    items.some((i) => location.pathname.startsWith(i.path));

  const defaultOpen = (id: string) => groups[id] ?? isActiveIn(
    sidebarGroups.find((g) => g.id === id)?.items ?? [],
  );

  return (
    <aside className={`flex flex-col border-r border-primary/10 bg-[#1d1b1b] text-white transition-all ${collapsed ? 'w-20' : 'w-72'}`}>
      <div className={`flex items-center border-b border-white/10 px-4 py-5 ${collapsed ? 'justify-center' : 'justify-between'}`}>
        {!collapsed && (
          <div>
            <p className="text-xs uppercase tracking-[0.35em] text-accent">GrandStay Hotel</p>
            <h1 className="mt-1 text-xl font-primary">Admin Console</h1>
          </div>
        )}
        <button onClick={toggleSidebar} className="rounded-lg p-2 text-white/50 hover:bg-white/10 hover:text-white">
          {collapsed ? '☰' : '✕'}
        </button>
      </div>

      <nav className={`flex-1 space-y-1 overflow-y-auto px-3 py-4 ${collapsed ? 'overflow-hidden' : ''}`}>
        {sidebarGroups.map((group) => {
          const open = defaultOpen(group.id);
          const groupActive = isActiveIn(group.items);

          return (
            <div key={group.id}>
              <button
                onClick={() => toggleGroup(group.id)}
                className={`flex w-full items-center rounded-lg px-3 py-3 text-sm font-medium transition ${
                  groupActive
                    ? 'bg-accent/20 text-accent'
                    : 'text-white/60 hover:bg-white/10 hover:text-white'
                } ${collapsed ? 'justify-center' : ''}`}
              >
                <GroupIcon icon={group.icon} />
                {!collapsed && (
                  <>
                    <span className="flex-1 text-left uppercase tracking-[0.15em] text-xs">{group.label}</span>
                    <span className={`transition ${open ? 'rotate-180' : ''}`}>▾</span>
                  </>
                )}
              </button>
              {open && !collapsed && (
                <div className="ml-2 mt-1 space-y-0.5 border-l border-white/10 pl-3">
                  {group.items
                    .filter((it) => !it.roles || (userRole && it.roles.includes(userRole)))
                    .map((item) => {
                    const isItemActive = location.pathname === item.path;
                    return (
                      <NavLink
                        key={item.path}
                        to={item.path}
                        className={`block rounded-lg px-3 py-2 text-sm transition ${
                          isItemActive
                            ? 'bg-accent text-white font-semibold'
                            : 'text-white/50 hover:bg-white/10 hover:text-white'
                        }`}
                      >
                        {item.title}
                      </NavLink>
                    );
                  })}
                </div>
              )}
            </div>
          );
        })}
      </nav>

      {!collapsed && (
        <div className="border-t border-white/10 px-4 py-4">
          <div className="flex items-center gap-3">
            <span className={`h-2.5 w-2.5 rounded-full ${connected ? 'bg-emerald-400' : 'bg-rose-400'}`} />
            <div className="text-xs">
              <p className="font-medium text-white/80">{connected ? 'Connected' : 'Disconnected'}</p>
              <p className="text-white/40">SignalR</p>
            </div>
          </div>
        </div>
      )}
    </aside>
  );
}
