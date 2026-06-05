import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useDashboardStore } from '../store/dashboardStore';
import { useRealtimeNotifications } from '../hooks/useRealtimeNotifications';
import { useNotificationStore } from '../store/notificationStore';
import MetricCard from '../components/MetricCard';
import SectionPanel from '../components/SectionPanel';
import { BarChart, DonutChart } from '../components/Charts';
import DataTable from '../components/DataTable';
import StatusBadge from '../components/StatusBadge';
import type { BookingRecord, RoomOverview, MaintenanceTicket } from '../../types';

export default function AdminDashboard() {
  const navigate = useNavigate();
  useRealtimeNotifications();
  const { metrics, occupancy, revenue, load } = useDashboardStore();
  const notifItems = useNotificationStore((s) => s.items);

  const [rooms, setRooms] = useState<RoomOverview[]>([]);
  const [bookings, setBookings] = useState<BookingRecord[]>([]);
  const [maintenance, setMaintenance] = useState<MaintenanceTicket[]>([]);

  useEffect(() => {
    load().catch(() => undefined);
    import('../../api').then(async (api) => {
      try {
        const [r, b, m] = await Promise.all([
          api.fetchRoomsOverview(),
          api.fetchBookingsAdmin(),
          api.fetchMaintenanceTickets(),
        ]);
        setRooms(r ?? []);
        setBookings(b ?? []);
        setMaintenance(m ?? []);
      } catch { /* ignore */ }
    });
  }, [load]);

  const roomStatusData = (() => {
    const counts: Record<string, number> = {};
    rooms.forEach((r) => { counts[r.status] = (counts[r.status] ?? 0) + 1; });
    return Object.entries(counts).map(([label, value], i) => ({
      label,
      value,
      color: ['#a37d4c', '#6b8c5e', '#5b7e9e', '#b56b6b', '#8a7aa3'][i % 5],
    }));
  })();

  return (
    <div className="px-6 py-8">
      <div className="mb-8 flex items-start justify-between">
        <div>
          <p className="text-xs uppercase tracking-[0.35em] text-accent">GrandStay Hotel</p>
          <h1 className="mt-2 text-2xl font-bold text-primary">Admin Dashboard</h1>
          <p className="mt-1 text-sm text-primary/50">Live telemetry for every wing of the hotel</p>
        </div>
        <button
          onClick={() => navigate('/')}
          className="rounded-lg border border-primary/10 px-4 py-2 text-xs font-semibold uppercase tracking-[0.16em] text-primary/60 hover:bg-primary/5 hover:text-primary"
        >
          Back to Home
        </button>
      </div>

      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        {metrics.map((m) => (
          <MetricCard key={m.label} metric={m} />
        ))}
      </div>

      <div className="mt-6 grid gap-6">
        <div className="rounded-2xl border border-primary/10 bg-white p-4">
          <p className="text-xs uppercase tracking-[0.28em] text-primary/40">System Health</p>
          <div className="mt-3 space-y-3">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium">PostgreSQL</p>
                <p className="text-xs text-primary/50">Primary DB</p>
              </div>
              <StatusBadge status="Connected" />
            </div>

            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium">RabbitMQ</p>
                <p className="text-xs text-primary/50">Message broker</p>
              </div>
              <StatusBadge status="Healthy" />
            </div>

            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium">SignalR</p>
                <p className="text-xs text-primary/50">Realtime hub</p>
              </div>
              <StatusBadge status="Online" />
            </div>

            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium">API Gateway</p>
                <p className="text-xs text-primary/50">Gateway & auth</p>
              </div>
              <StatusBadge status="Degraded" />
            </div>
          </div>
        </div>
      </div>

      <div className="mt-8 grid gap-6 lg:grid-cols-2">
        <SectionPanel title="Occupancy Trend" subtitle="7-day room occupancy">
          <BarChart
            data={(occupancy.length ? occupancy : [
              { label: 'Mon', value: 72 }, { label: 'Tue', value: 78 },
              { label: 'Wed', value: 85 }, { label: 'Thu', value: 91 },
              { label: 'Fri', value: 96 }, { label: 'Sat', value: 88 },
              { label: 'Sun', value: 74 },
            ]).map((d) => ({ ...d, color: '#a37d4c' }))}
          />
        </SectionPanel>

        <SectionPanel title="Room Status" subtitle="Current occupancy breakdown">
          {roomStatusData.length > 0 ? (
            <DonutChart data={roomStatusData} />
          ) : (
            <DonutChart
              data={[
                { label: 'Occupied', value: 120, color: '#a37d4c' },
                { label: 'Available', value: 45, color: '#6b8c5e' },
                { label: 'Dirty', value: 18, color: '#b56b6b' },
                { label: 'Maintenance', value: 7, color: '#5b7e9e' },
              ]}
            />
          )}
        </SectionPanel>
      </div>

      <div className="mt-8 grid gap-6 lg:grid-cols-2">
        <SectionPanel title="Revenue" subtitle="Monthly revenue (USD)">
          <BarChart
            data={(revenue.length ? revenue : [
              { label: 'Jan', value: 42500 }, { label: 'Feb', value: 38200 },
              { label: 'Mar', value: 46100 }, { label: 'Apr', value: 52300 },
              { label: 'May', value: 58700 }, { label: 'Jun', value: 61400 },
            ]).map((d) => ({ ...d, color: '#6b8c5e' }))}
            height={180}
          />
        </SectionPanel>

        <SectionPanel title="Live Notifications" subtitle="Latest SignalR events">
          <div className="max-h-64 space-y-3 overflow-y-auto">
            {(notifItems.length ? notifItems : []).map((n) => (
              <div key={n.id} className="rounded-xl border border-primary/10 p-4">
                <div className="flex items-center justify-between gap-4">
                  <strong className="text-sm">{n.title}</strong>
                  <StatusBadge status={n.type} />
                </div>
                <p className="mt-1 text-sm text-primary/60">{n.message}</p>
              </div>
            ))}
            {notifItems.length === 0 && (
              <p className="py-8 text-center text-sm text-primary/40">No live notifications yet</p>
            )}
          </div>
        </SectionPanel>
      </div>

      <div className="mt-8">
        <SectionPanel title="Latest Bookings" subtitle="Recent check-ins and reservations">
          <DataTable
            columns={[
              { key: 'guestName', label: 'Guest', sortable: true },
              { key: 'roomNumber', label: 'Room', sortable: true },
              { key: 'status', label: 'Status', sortable: true, render: (r) => <StatusBadge status={r.status} /> },
              { key: 'checkInDate', label: 'Check In', sortable: true },
              { key: 'checkOutDate', label: 'Check Out', sortable: true },
              { key: 'total', label: 'Total', sortable: true, render: (r) => `$${r.total?.toLocaleString() ?? 0}` },
            ]}
            data={bookings.slice(0, 5)}
            keyExtractor={(r) => r.id}
            searchable={false}
            pageSize={5}
          />
        </SectionPanel>
      </div>

      <div className="mt-6 grid gap-6 lg:grid-cols-2">
        <SectionPanel title="Active Maintenance" subtitle="Open work orders">
          <DataTable
            columns={[
              { key: 'title', label: 'Issue', sortable: true },
              { key: 'priority', label: 'Priority', sortable: true, render: (r) => <StatusBadge status={r.priority} /> },
              { key: 'status', label: 'Status', sortable: true },
              { key: 'technician', label: 'Technician', sortable: true },
              { key: 'roomNumber', label: 'Room', sortable: true },
            ]}
            data={maintenance.slice(0, 5)}
            keyExtractor={(r) => r.id}
            searchable={false}
            pageSize={5}
          />
        </SectionPanel>

        <SectionPanel title="Room Overview" subtitle="All rooms at a glance">
          <DataTable
            columns={[
              { key: 'roomNumber', label: 'Room', sortable: true, render: (r) => `#${r.roomNumber}` },
              { key: 'status', label: 'Status', sortable: true, render: (r) => <StatusBadge status={r.status} /> },
              { key: 'guestName', label: 'Guest', sortable: true },
              { key: 'housekeeping', label: 'Housekeeping', sortable: true },
            ]}
            data={rooms.slice(0, 5)}
            keyExtractor={(r) => r.id}
            searchable={false}
            pageSize={5}
          />
        </SectionPanel>
      </div>
    </div>
  );
}
