import GenericListPage from './GenericListPage';
import { useNotificationStore } from '../store/notificationStore';

export default function NotificationsPage() {
  const notifications = useNotificationStore((s) => s.items);
  const markRead = useNotificationStore((s) => s.markRead);

  const severityColor: Record<string, string> = {
    info: 'bg-blue-100 text-blue-800',
    warning: 'bg-amber-100 text-amber-800',
    critical: 'bg-red-100 text-red-800',
    success: 'bg-emerald-100 text-emerald-800',
  };

  return (
    <GenericListPage title="Notifications" eyebrow="Realtime">
      {notifications.length === 0 ? (
        <p className="text-sm text-primary/50">No notifications.</p>
      ) : (
        <div className="space-y-3">
          {notifications.map((n) => (
            <div key={n.id} className={`flex items-start gap-4 rounded border border-primary/10 bg-white p-4 shadow-sm ${n.read ? 'opacity-60' : ''}`}>
              <span className={`mt-0.5 rounded px-2 py-0.5 text-xs font-medium ${severityColor[n.type] ?? 'bg-gray-100 text-gray-800'}`}>
                {n.type.toUpperCase()}
              </span>
              <div className="flex-1">
                <h3 className="font-semibold">{n.title}</h3>
                <p className="mt-1 text-sm text-primary/50">{n.message}</p>
              </div>
              <span className="text-xs text-primary/40">{n.time}</span>
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
