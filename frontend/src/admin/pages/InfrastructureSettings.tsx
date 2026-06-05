import { NavLink } from 'react-router-dom';
import GenericListPage from './GenericListPage';

export default function InfrastructureSettings() {
  return (
    <GenericListPage eyebrow="Settings" title="Infrastructure">
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <NavLink to="rabbitmq" className="rounded border border-primary/10 bg-white p-4 text-center hover:shadow">
          <p className="text-sm font-semibold">RabbitMQ Monitor</p>
          <p className="text-xs text-primary/50 mt-2">Queue and consumer overview</p>
        </NavLink>

        <NavLink to="database" className="rounded border border-primary/10 bg-white p-4 text-center hover:shadow">
          <p className="text-sm font-semibold">Database Monitor</p>
          <p className="text-xs text-primary/50 mt-2">Postgres status and connections</p>
        </NavLink>

        <NavLink to="websockets" className="rounded border border-primary/10 bg-white p-4 text-center hover:shadow">
          <p className="text-sm font-semibold">WebSocket Connections</p>
          <p className="text-xs text-primary/50 mt-2">SignalR hub overview</p>
        </NavLink>

        <NavLink to="event-logs" className="rounded border border-primary/10 bg-white p-4 text-center hover:shadow">
          <p className="text-sm font-semibold">Event Logs</p>
          <p className="text-xs text-primary/50 mt-2">Event routing and delivery</p>
        </NavLink>
      </div>
    </GenericListPage>
  );
}
