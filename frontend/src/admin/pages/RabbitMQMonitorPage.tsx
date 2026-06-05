import GenericListPage from './GenericListPage';

export default function RabbitMQMonitorPage() {
  return (
    <GenericListPage title="RabbitMQ Monitor" eyebrow="Infrastructure">
      <div className="rounded border border-primary/10 bg-white p-6 shadow-sm">
        <h2 className="text-lg font-semibold">Message Queue Status</h2>
        <div className="mt-4 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          <div className="rounded border border-primary/5 bg-gray-50 p-4">
            <p className="text-xs uppercase tracking-widest text-primary/40">Queues</p>
            <p className="mt-1 text-2xl font-bold">7</p>
          </div>
          <div className="rounded border border-primary/5 bg-gray-50 p-4">
            <p className="text-xs uppercase tracking-widest text-primary/40">Consumers</p>
            <p className="mt-1 text-2xl font-bold">3</p>
          </div>
          <div className="rounded border border-primary/5 bg-gray-50 p-4">
            <p className="text-xs uppercase tracking-widest text-primary/40">Messages/s</p>
            <p className="mt-1 text-2xl font-bold">42</p>
          </div>
        </div>
        <div className="mt-6">
          <h3 className="text-sm font-semibold uppercase tracking-wider text-primary/50">Active Queues</h3>
          <div className="mt-3 space-y-2">
            {['hotelos.events', 'hotelos.events.retry', 'hotelos.events.dlq', 'hotelos.websocket.events'].map((q) => (
              <div key={q} className="flex items-center justify-between rounded border border-primary/5 bg-gray-50 px-4 py-2">
                <span className="font-mono text-sm">{q}</span>
                <span className="text-xs text-emerald-600">Active</span>
              </div>
            ))}
          </div>
        </div>
      </div>
    </GenericListPage>
  );
}
