import GenericListPage from './GenericListPage';

export default function ActivityFeedPage() {
  const activities = [
    { id: 1, action: 'Guest checked in', detail: 'Room 101 - Amelia Stone', time: '2 min ago' },
    { id: 2, action: 'Payment received', detail: 'INV-10021 - $420.00', time: '5 min ago' },
    { id: 3, action: 'Room cleaned', detail: 'Room 205 marked as Clean', time: '8 min ago' },
    { id: 4, action: 'Maintenance resolved', detail: 'Room 302 AC repair completed', time: '12 min ago' },
    { id: 5, action: 'Booking created', detail: 'Room 207 - Daniel Reed', time: '15 min ago' },
    { id: 6, action: 'Food order delivered', detail: 'Room 101 - Breakfast set', time: '18 min ago' },
  ];

  return (
    <GenericListPage title="Activity Feed" eyebrow="Realtime">
      <div className="space-y-3">
        {activities.map((a) => (
          <div key={a.id} className="flex items-center gap-4 rounded border border-primary/10 bg-white p-4 shadow-sm">
            <div className="h-2 w-2 rounded-full bg-accent" />
            <div className="flex-1">
              <h3 className="font-semibold">{a.action}</h3>
              <p className="text-sm text-primary/50">{a.detail}</p>
            </div>
            <span className="text-xs text-primary/40">{a.time}</span>
          </div>
        ))}
      </div>
    </GenericListPage>
  );
}
