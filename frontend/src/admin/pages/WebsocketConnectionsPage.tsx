import GenericListPage from './GenericListPage';

export default function WebsocketConnectionsPage() {
  return (
    <GenericListPage title="WebSocket Connections" eyebrow="Realtime">
      <div className="rounded border border-primary/10 bg-white p-6 shadow-sm">
        <h2 className="text-lg font-semibold">SignalR Hub Status</h2>
        <div className="mt-4 grid gap-4 sm:grid-cols-2">
          <div className="rounded border border-primary/5 bg-gray-50 p-4">
            <p className="text-xs uppercase tracking-widest text-primary/40">Active Connections</p>
            <p className="mt-1 text-2xl font-bold">12</p>
          </div>
          <div className="rounded border border-primary/5 bg-gray-50 p-4">
            <p className="text-xs uppercase tracking-widest text-primary/40">Messages Sent</p>
            <p className="mt-1 text-2xl font-bold">1,847</p>
          </div>
        </div>
        <div className="mt-6">
          <h3 className="text-sm font-semibold uppercase tracking-wider text-primary/50">Connected Clients</h3>
          <div className="mt-3 space-y-2">
            {['Admin Dashboard', 'Reception Terminal', 'Housekeeping Tablet', 'Kitchen Display'].map((client) => (
              <div key={client} className="flex items-center justify-between rounded border border-primary/5 bg-gray-50 px-4 py-2">
                <span className="text-sm">{client}</span>
                <span className="text-xs text-emerald-600">Connected</span>
              </div>
            ))}
          </div>
        </div>
      </div>
    </GenericListPage>
  );
}
