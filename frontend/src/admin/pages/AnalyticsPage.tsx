import GenericListPage from './GenericListPage';

export default function AnalyticsPage() {
  return (
    <GenericListPage title="Analytics" eyebrow="Insights">
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {[
          { label: 'Total Revenue', value: '$128,420', change: '+12%' },
          { label: 'Avg Occupancy', value: '86%', change: '+5%' },
          { label: 'Avg Daily Rate', value: '$189', change: '+8%' },
          { label: 'RevPAR', value: '$162', change: '+7%' },
          { label: 'Total Bookings', value: '1,247', change: '+15%' },
          { label: 'Avg Stay', value: '3.2 nights', change: '+0.3' },
          { label: 'Guest Satisfaction', value: '4.7/5', change: '+0.2' },
          { label: 'Repeat Guests', value: '34%', change: '+4%' },
        ].map((item) => (
          <div key={item.label} className="rounded border border-primary/10 bg-white p-4 shadow-sm">
            <p className="text-xs uppercase tracking-widest text-primary/40">{item.label}</p>
            <p className="mt-1 text-2xl font-bold">{item.value}</p>
            <p className="mt-1 text-xs text-emerald-600">{item.change}</p>
          </div>
        ))}
      </div>
    </GenericListPage>
  );
}
