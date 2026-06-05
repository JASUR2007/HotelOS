import { useEffect, useState } from 'react';
import GenericListPage from './GenericListPage';
import { fetchNotifications, getHotelApiBaseUrl } from '../../api';

interface Notification {
  id: string | number;
  title: string;
  message: string;
  severity: string;
  createdAt: string;
  read?: boolean;
}

export default function NotificationsPage() {
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchNotifications()
      .then((data) => {
        const mapped = (data as any[]).map((n: any) => ({
          id: n.id ?? n.createdAt,
          title: n.title ?? '',
          message: n.message ?? '',
          severity: n.type === 'critical' || n.type === 'maintenance' ? 'critical' :
                    n.type === 'warning' ? 'warning' : 'info',
          createdAt: n.createdAt ?? '',
        }));
        setNotifications(mapped);
      })
      .catch(() => setNotifications([]))
      .finally(() => setLoading(false));
  }, []);

  const severityColor: Record<string, string> = {
    info: 'bg-blue-100 text-blue-800',
    warning: 'bg-amber-100 text-amber-800',
    critical: 'bg-red-100 text-red-800',
  };

  async function markRead(id: string | number) {
    setNotifications((prev) => prev.map((n) => (n.id === id ? { ...n, read: true } : n)));
    try {
      const res = await fetch(`${getHotelApiBaseUrl()}/notifications/${id}/read`, { method: 'PUT' });
      if (!res.ok) throw new Error('Failed');
    } catch { /* ignore */ }
  }

  return (
    <GenericListPage title="Notifications" eyebrow="Realtime">
      {loading ? (
        <div className="flex justify-center py-10">
          <div className="h-8 w-8 animate-spin rounded-full border-4 border-accent border-t-transparent" />
        </div>
      ) : notifications.length === 0 ? (
        <p className="text-sm text-primary/50">No notifications.</p>
      ) : (
        <div className="space-y-3">
          {notifications.map((n) => (
            <div key={n.id} className={`flex items-start gap-4 rounded border border-primary/10 bg-white p-4 shadow-sm ${n.read ? 'opacity-60' : ''}`}>
              <span className={`mt-0.5 rounded px-2 py-0.5 text-xs font-medium ${severityColor[n.severity] ?? 'bg-gray-100 text-gray-800'}`}>
                {n.severity}
              </span>
              <div className="flex-1">
                <h3 className="font-semibold">{n.title}</h3>
                <p className="mt-1 text-sm text-primary/50">{n.message}</p>
              </div>
              <span className="text-xs text-primary/40">{n.createdAt}</span>
              <button
                onClick={() => markRead(n.id)}
                disabled={n.read}
                className="rounded-lg border border-primary/10 px-3 py-1.5 text-xs font-medium hover:bg-primary/5 disabled:cursor-not-allowed disabled:opacity-50"
              >
                {n.read ? 'Read ✓' : 'Mark read'}
              </button>
            </div>
          ))}
        </div>
      )}
    </GenericListPage>
  );
}
