import GenericListPage from './GenericListPage';

export default function SystemHealthPage() {
  return (
    <GenericListPage title="System Health" eyebrow="Monitoring">
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {[
          { name: 'Gateway API', port: 8080, status: 'Healthy' },
          { name: 'User Service', port: 8086, status: 'Healthy' },
          { name: 'Reception Service', port: 8082, status: 'Healthy' },
          { name: 'Room Service', port: 8084, status: 'Healthy' },
          { name: 'Housekeeping Service', port: 8083, status: 'Healthy' },
          { name: 'Maintenance Service', port: 8085, status: 'Healthy' },
          { name: 'Payment Service', port: 8087, status: 'Healthy' },
          { name: 'WebSocket Service', port: 8081, status: 'Healthy' },
          { name: 'PostgreSQL', port: 5432, status: 'Healthy' },
          { name: 'RabbitMQ', port: 5672, status: 'Healthy' },
        ].map((svc) => (
          <div key={svc.name} className="rounded border border-primary/10 bg-white p-4 shadow-sm">
            <div className="flex items-center justify-between">
              <h3 className="font-semibold">{svc.name}</h3>
              <span className="rounded bg-emerald-100 px-2 py-0.5 text-xs font-medium text-emerald-800">
                {svc.status}
              </span>
            </div>
            <p className="mt-1 text-xs text-primary/40">Port {svc.port}</p>
          </div>
        ))}
      </div>
    </GenericListPage>
  );
}
