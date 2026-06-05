import { useState, useRef, useEffect, type FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../../store/authStore';
import { useWebsocketStatus } from '../hooks/useWebsocketStatus';
import { useNotificationStore } from '../store/notificationStore';
import { fetchNotifications } from '../../api';
import { Bell, Circle, Search } from '../../components/SimpleIcons';

export default function Header() {
  const navigate = useNavigate();
  const user = useAuthStore((s) => s.user);
  const logout = useAuthStore((s) => s.logout);
  const { connected, reconnecting } = useWebsocketStatus();
  const notifications = useNotificationStore((s) => s.items);
  const unreadCount = useNotificationStore((s) => s.unreadCount);
  const markRead = useNotificationStore((s) => s.markRead);
  const markAllRead = useNotificationStore((s) => s.markAllRead);
  const addNotification = useNotificationStore((s) => s.addNotification);
  const clearAll = useNotificationStore((s) => s.clearAll);

  const [searchQuery, setSearchQuery] = useState('');
  const [notifOpen, setNotifOpen] = useState(false);
  const [userMenuOpen, setUserMenuOpen] = useState(false);
  const notifRef = useRef<HTMLDivElement>(null);
  const userRef = useRef<HTMLDivElement>(null);

  function handleSearch(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const query = searchQuery.trim();
    if (!query) return;

    const lower = query.toLowerCase();
    let path = '/admin/dashboard';
    if (lower.includes('room')) path = '/admin/rooms';
    if (lower.includes('guest')) path = '/admin/bookings';
    if (lower.includes('booking') || lower.includes('reservation')) path = '/admin/bookings';
    if (lower.includes('order') || lower.includes('food')) path = '/admin/orders';
    if (lower.includes('maintenance') || lower.includes('ticket')) path = '/admin/maintenance';
    if (lower.includes('clean') || lower.includes('housekeeping')) path = '/admin/housekeeping';
    if (lower.includes('payment') || lower.includes('invoice')) path = '/admin/payments';
    if (lower.includes('user') || lower.includes('role') || lower.includes('permission')) path = '/admin/users';
    if (lower.includes('notification')) path = '/admin/notifications';

    navigate(`${path}?q=${encodeURIComponent(query)}`);
  }

  useEffect(() => {
    function handleClick(e: MouseEvent) {
      if (notifRef.current && !notifRef.current.contains(e.target as Node)) setNotifOpen(false);
      if (userRef.current && !userRef.current.contains(e.target as Node)) setUserMenuOpen(false);
    }
    document.addEventListener('mousedown', handleClick);
    return () => document.removeEventListener('mousedown', handleClick);
  }, []);

  useEffect(() => {
    fetchNotifications()
      .then((data) => {
        clearAll();
        (data as any[]).forEach((n: any) => {
          addNotification({
            id: String(n.id ?? n.createdAt),
            title: n.title ?? '',
            message: n.message ?? '',
            type: (n.type === 'maintenance' || n.type === 'critical') ? 'critical' :
                  n.type === 'warning' ? 'warning' :
                  n.type === 'success' ? 'success' : 'info',
            time: n.createdAt ?? '',
            read: false,
          });
        });
      })
      .catch(() => {});
  }, []);

  return (
    <header className="sticky top-0 z-20 border-b border-primary/10 bg-white/95 backdrop-blur shadow-sm">
      <div className="flex items-center justify-between px-6 py-3">
        <div className="flex items-center gap-4">
          <button
            onClick={() => navigate('/')}
            className="rounded-lg border border-primary/10 px-3 py-2 text-xs font-semibold uppercase tracking-[0.16em] text-primary/60 hover:bg-primary/5 hover:text-primary"
          >
            Home
          </button>
          <div className="hidden border-l border-primary/10 pl-4 lg:block">
            <p className="text-xs uppercase tracking-[0.24em] text-accent">GrandStay Hotel</p>
            <p className="text-sm font-semibold text-primary">Admin Dashboard</p>
          </div>
          <form onSubmit={handleSearch} className="relative">
            <input
              type="text"
              placeholder="Search rooms, guests, bookings..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="w-72 rounded-lg border border-primary/10 bg-[#f5efe7] px-4 py-2 pl-10 text-sm text-primary outline-none focus:border-accent focus:bg-white"
            />
            <span className="absolute left-3 top-1/2 -translate-y-1/2 text-primary/30"><Search className="inline-block" /></span>
          </form>
        </div>

        <div className="flex items-center gap-3">
          

          <span className={`flex items-center gap-1.5 rounded-full px-3 py-1 text-xs font-medium ${
            connected ? 'bg-emerald-50 text-emerald-700' : reconnecting ? 'bg-amber-50 text-amber-700' : 'bg-rose-50 text-rose-700'
          }`}>
            <span className={`h-2 w-2 rounded-full ${connected ? 'bg-emerald-500' : reconnecting ? 'bg-amber-500' : 'bg-rose-500'}`} />
            {connected ? 'LIVE' : reconnecting ? 'RECONNECT' : 'OFF'}
          </span>

          <div ref={notifRef} className="relative">
            <button
              onClick={() => setNotifOpen(!notifOpen)}
              className="relative rounded-lg p-2 text-primary/50 hover:bg-primary/5 hover:text-primary"
            >
              <Bell className="inline-block" />
              {unreadCount > 0 && (
                <span className="absolute -right-1 -top-1 flex h-5 w-5 items-center justify-center rounded-full bg-rose-500 text-[10px] font-bold text-white">
                  {unreadCount > 9 ? '9+' : unreadCount}
                </span>
              )}
            </button>

            {notifOpen && (
              <div className="absolute right-0 top-full mt-2 w-96 rounded-xl border border-primary/10 bg-white shadow-xl">
                <div className="flex items-center justify-between border-b border-primary/10 px-5 py-4">
                  <h3 className="font-semibold text-primary">Notifications</h3>
                  <button onClick={markAllRead} className="text-xs text-accent hover:underline">Mark all read</button>
                </div>
                <div className="max-h-80 overflow-y-auto">
                  {notifications.length === 0 ? (
                    <p className="px-5 py-8 text-center text-sm text-primary/40">No notifications</p>
                  ) : (
                    notifications.slice(0, 8).map((n) => (
                      <button
                        key={n.id}
                        onClick={() => markRead(n.id)}
                        className={`flex w-full gap-3 border-b border-primary/5 px-5 py-3 text-left transition hover:bg-primary/5 ${n.read ? 'opacity-60' : ''}`}
                      >
                        <span className="mt-0.5 text-lg">
                          <Circle className={`h-3 w-3 ${n.type === 'critical' ? 'text-rose-500' : n.type === 'warning' ? 'text-amber-500' : n.type === 'success' ? 'text-emerald-500' : 'text-primary/50'}`} />
                        </span>
                        <div className="flex-1">
                          <p className="text-sm font-medium text-primary">{n.title}</p>
                          <p className="text-xs text-primary/50">{n.message}</p>
                        </div>
                        <span className="text-xs text-primary/40">{n.time}</span>
                      </button>
                    ))
                  )}
                </div>
                <div className="border-t border-primary/10 px-5 py-3">
                  <button
                    onClick={() => { setNotifOpen(false); navigate('/admin/notifications'); }}
                    className="w-full text-center text-xs text-accent hover:underline"
                  >
                    View all notifications
                  </button>
                </div>
              </div>
            )}
          </div>

          <div ref={userRef} className="relative">
            <button
              onClick={() => setUserMenuOpen(!userMenuOpen)}
              className="flex items-center gap-2 rounded-lg border border-primary/10 px-4 py-2 text-sm transition hover:bg-primary/5"
            >
              <span className="flex h-7 w-7 items-center justify-center rounded-full bg-accent text-xs font-bold text-white">
                {user?.displayName?.charAt(0) ?? 'A'}
              </span>
              <div className="text-left">
                <p className="text-sm font-medium text-primary">{user?.displayName ?? 'Admin'}</p>
                <p className="text-xs text-primary/40">{user?.role ?? 'Guest'}</p>
              </div>
              <span className="text-primary/30">▾</span>
            </button>

            {userMenuOpen && (
              <div className="absolute right-0 top-full mt-2 w-56 rounded-xl border border-primary/10 bg-white shadow-xl">
                <div className="border-b border-primary/10 px-5 py-4">
                  <p className="text-sm font-medium text-primary">{user?.displayName}</p>
                  <p className="text-xs text-primary/40">{user?.email}</p>
                </div>
                <div className="p-2">
                  <button className="flex w-full items-center gap-3 rounded-lg px-3 py-2 text-sm text-primary/70 hover:bg-primary/5 hover:text-primary">
                     Profile
                  </button>
                  <button className="flex w-full items-center gap-3 rounded-lg px-3 py-2 text-sm text-primary/70 hover:bg-primary/5 hover:text-primary">
                    Account Settings
                  </button>
                </div>
                <div className="border-t border-primary/10 p-2">
                  <button
                    onClick={logout}
                    className="flex w-full items-center gap-3 rounded-lg px-3 py-2 text-sm text-rose-600 hover:bg-rose-50"
                  >
                    🚪 Sign Out
                  </button>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </header>
  );
}
