import GenericListPage from './GenericListPage';

export default function DatabaseMonitorPage() {
  return (
    <GenericListPage title="Database Monitor" eyebrow="Infrastructure">
      <div className="rounded border border-primary/10 bg-white p-6 shadow-sm">
        <h2 className="text-lg font-semibold">PostgreSQL Status</h2>
        <div className="mt-4 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          <div className="rounded border border-primary/5 bg-gray-50 p-4">
            <p className="text-xs uppercase tracking-widest text-primary/40">Databases</p>
            <p className="mt-1 text-2xl font-bold">8</p>
          </div>
          <div className="rounded border border-primary/5 bg-gray-50 p-4">
            <p className="text-xs uppercase tracking-widest text-primary/40">Connections</p>
            <p className="mt-1 text-2xl font-bold">24</p>
          </div>
          <div className="rounded border border-primary/5 bg-gray-50 p-4">
            <p className="text-xs uppercase tracking-widest text-primary/40">Uptime</p>
            <p className="mt-1 text-2xl font-bold">99.9%</p>
          </div>
        </div>
        <div className="mt-6">
          <h3 className="text-sm font-semibold uppercase tracking-wider text-primary/50">Schemas</h3>
          <div className="mt-3 space-y-2">
            {['hotelos_users', 'hotelos_reception', 'hotelos_housekeeping', 'hotelos_room', 'hotelos_maintenance', 'hotelos_payments', 'hotelos_websocket', 'hotelos_gateway'].map((db) => (
              <div key={db} className="flex items-center justify-between rounded border border-primary/5 bg-gray-50 px-4 py-2">
                <span className="font-mono text-sm">{db}</span>
                <span className="text-xs text-emerald-600">Connected</span>
              </div>
            ))}
          </div>
        </div>
      </div>
    </GenericListPage>
  );
}
