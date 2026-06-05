import GenericListPage from './GenericListPage';

export default function RevenuePage() {
  return (
    <GenericListPage title="Revenue" eyebrow="Insights">
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {[
          { label: 'Room Revenue', value: '$98,200', pct: '76%' },
          { label: 'F&B Revenue', value: '$18,420', pct: '14%' },
          { label: 'Minibar', value: '$4,800', pct: '4%' },
          { label: 'Other Services', value: '$7,000', pct: '6%' },
        ].map((item) => (
          <div key={item.label} className="rounded border border-primary/10 bg-white p-4 shadow-sm">
            <p className="text-xs uppercase tracking-widest text-primary/40">{item.label}</p>
            <p className="mt-1 text-2xl font-bold">{item.value}</p>
            <div className="mt-2 h-2 w-full rounded-full bg-gray-100">
              <div className="h-2 rounded-full bg-accent" style={{ width: item.pct }} />
            </div>
            <p className="mt-1 text-xs text-primary/40">{item.pct} of total</p>
          </div>
        ))}
      </div>
    </GenericListPage>
  );
}
