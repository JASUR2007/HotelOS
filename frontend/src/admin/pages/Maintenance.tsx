import { useEffect, useState } from 'react';
import DataTable from '../components/DataTable';
import Modal, { ConfirmDeleteModal } from '../components/Modal';
import StatusBadge from '../components/StatusBadge';
import { getHotelApiBaseUrl, fetchMaintenanceTickets, fetchRoomsOverview, fetchUsers } from '../../api';
import type { AuthUser, RoomOverview } from '../../types';

function getAccessToken(): string {
  const store = (window as unknown as Record<string, unknown>).__hotelos_auth_store__ as
    | { getState(): { accessToken?: string } }
    | undefined;
  return store?.getState().accessToken ?? '';
}

interface MaintenanceTicket {
  id: number;
  roomNumber: string;
  title: string;
  description?: string;
  priority: string;
  status: string;
  technician?: string;
}

const PRIORITIES = ['Low', 'Medium', 'High', 'Critical'];
const STATUSES = ['Queued', 'Assigned', 'In Progress', 'Resolved', 'Closed'];

const emptyCreateForm = { roomNumber: '', title: '', description: '', priority: 'Medium', technician: '' };
const emptyEditForm = { status: 'Queued', technician: '', priority: 'Medium' };

function isTechnician(user: AuthUser) {
  return user.role === 'Technician';
}

export default function Maintenance() {
  const [tickets, setTickets] = useState<MaintenanceTicket[]>([]);
  const [users, setUsers] = useState<AuthUser[]>([]);
  const [rooms, setRooms] = useState<RoomOverview[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modal, setModal] = useState<'create' | 'edit' | null>(null);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<MaintenanceTicket | null>(null);
  const [createForm, setCreateForm] = useState(emptyCreateForm);
  const [editForm, setEditForm] = useState(emptyEditForm);
  const [saving, setSaving] = useState(false);

  const apiBaseUrl = getHotelApiBaseUrl();

  async function loadTickets() {
    setLoading(true);
    setError(null);
    try {
      const [ticketsData, usersData, roomData] = await Promise.all([
        fetchMaintenanceTickets(),
        fetchUsers(),
        fetchRoomsOverview(),
      ]);
      setTickets(ticketsData);
      setUsers(usersData);
      setRooms(roomData);
    } catch {
      setError('Failed to load maintenance tickets.');
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { loadTickets(); }, []);

  function openCreate() {
    setCreateForm(emptyCreateForm);
    setModal('create');
  }

  function openEdit(ticket: MaintenanceTicket) {
    setEditForm({
      status: ticket.status,
      technician: ticket.technician ?? '',
      priority: ticket.priority,
    });
    setEditingId(ticket.id);
    setModal('edit');
  }

  async function handleCreate() {
    setSaving(true);
    try {
      const token = getAccessToken();
      const r = await fetch(`${apiBaseUrl}/maintenance`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          ...(token ? { Authorization: `Bearer ${token}` } : {}),
        },
        body: JSON.stringify({
          roomNumber: createForm.roomNumber,
          title: createForm.title,
          priority: createForm.priority,
        }),
      });
      if (!r.ok) throw new Error('Failed');
      const createdTicket = await r.json();

      if (createForm.technician) {
        const assignResponse = await fetch(`${apiBaseUrl}/maintenance/assign`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
          },
          body: JSON.stringify({ issueId: createdTicket.id, technicianName: createForm.technician }),
        });
        if (!assignResponse.ok) throw new Error('Failed to assign technician');
      }

      setModal(null);
      await loadTickets();
    } catch {
      setError('Failed to create maintenance ticket.');
    } finally {
      setSaving(false);
    }
  }

  async function handleEdit() {
    if (editingId == null) return;
    setSaving(true);
    try {
      const token = getAccessToken();
      const headers: Record<string, string> = {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      };
      let r = await fetch(`${apiBaseUrl}/maintenance/${editingId}`, {
          method: 'PUT',
          headers,
          body: JSON.stringify({
            status: editForm.status,
            technicianName: editForm.technician,
            priority: editForm.priority,
          }),
        });
      if (!r.ok) {
        r = await fetch(`${apiBaseUrl}/maintenance/${editingId}`, {
          method: 'PATCH',
          headers,
          body: JSON.stringify({
            status: editForm.status,
            technicianName: editForm.technician,
            priority: editForm.priority,
          }),
        });
        if (!r.ok) throw new Error('Failed');
      }
      setModal(null);
      await loadTickets();
    } catch {
      setError('Failed to update maintenance ticket.');
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete() {
    if (!deleteTarget) return;
    try {
      const token = getAccessToken();
      const r = await fetch(`${apiBaseUrl}/maintenance/${deleteTarget.id}`, {
        method: 'DELETE',
        headers: token ? { Authorization: `Bearer ${token}` } : {},
      });
      if (!r.ok) throw new Error('Failed');
      setDeleteTarget(null);
      await loadTickets();
    } catch {
      setError('Failed to delete maintenance ticket.');
    }
  }

  return (
    <div className="px-6 py-8">
      <div className="mb-8">
        <p className="text-xs uppercase tracking-[0.35em] text-accent">Maintenance</p>
        <h1 className="mt-2 text-2xl font-bold text-primary">Priority queue</h1>
        <p className="mt-1 text-sm text-primary/50">Track and manage facility maintenance tickets</p>
      </div>

      {error && (
        <div className="mb-4 rounded-xl border border-rose-200 bg-rose-50 p-3 text-sm text-rose-700">{error}</div>
      )}

      <DataTable
        columns={[
          { key: 'id', label: '#', sortable: true },
          { key: 'roomNumber', label: 'Room', sortable: true, render: (r: MaintenanceTicket) => <span className="font-semibold">#{r.roomNumber}</span> },
          { key: 'title', label: 'Title', sortable: true },
          { key: 'priority', label: 'Priority', sortable: true, render: (r: MaintenanceTicket) => <StatusBadge status={r.priority} /> },
          { key: 'status', label: 'Status', sortable: true, render: (r: MaintenanceTicket) => <StatusBadge status={r.status} /> },
          { key: 'technician', label: 'Technician', sortable: true, render: (r: MaintenanceTicket) => <span className="text-sm">{r.technician || '\u2014'}</span> },
        ]}
        data={tickets}
        keyExtractor={(r) => r.id}
        searchKeys={['roomNumber', 'title', 'status', 'technician', 'priority']}
        searchPlaceholder="Search tickets..."
        loading={loading}
        toolbar={
          <button
            onClick={openCreate}
            className="rounded-lg bg-accent px-5 py-2.5 text-sm font-medium text-white hover:bg-accent/90"
          >
            + New Ticket
          </button>
        }
        actions={(row) => (
          <>
            <button onClick={() => openEdit(row)} className="rounded-lg border border-primary/10 px-3 py-1.5 text-xs text-primary/60 hover:bg-primary/5">Edit</button>
            <button onClick={() => setDeleteTarget(row)} className="rounded-lg border border-rose-200 px-3 py-1.5 text-xs text-rose-600 hover:bg-rose-50">Delete</button>
          </>
        )}
      />

      <Modal open={modal === 'create'} onClose={() => setModal(null)} title="Create Maintenance Ticket">
        <div className="space-y-4">
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Room Number</label>
            <select value={createForm.roomNumber} onChange={(e) => setCreateForm({ ...createForm, roomNumber: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent">
              <option value="">Select room</option>
              {rooms.map((room) => (
                <option key={room.id} value={room.roomNumber}>#{room.roomNumber} ({room.status})</option>
              ))}
            </select>
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Title</label>
            <input value={createForm.title} onChange={(e) => setCreateForm({ ...createForm, title: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Description</label>
            <textarea value={createForm.description} onChange={(e) => setCreateForm({ ...createForm, description: e.target.value })} rows={3}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent resize-none" />
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Assigned Technician</label>
            <select value={createForm.technician} onChange={(e) => setCreateForm({ ...createForm, technician: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent">
              <option value="">— Select Technician —</option>
              {users.filter((u) => ['technician', 'housekeeper'].includes(u.role?.toLowerCase())).map(u => <option key={u.id} value={u.displayName}>{u.displayName} ({u.role})</option>)}
            </select>
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Priority</label>
            <select value={createForm.priority} onChange={(e) => setCreateForm({ ...createForm, priority: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent">
              {PRIORITIES.map((p) => <option key={p}>{p}</option>)}
            </select>
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <button onClick={() => setModal(null)} className="rounded-lg border border-primary/10 px-5 py-2.5 text-sm text-primary/60 hover:bg-primary/5">Cancel</button>
            <button onClick={handleCreate} disabled={saving}
              className="rounded-lg bg-accent px-5 py-2.5 text-sm text-white hover:bg-accent/90 disabled:opacity-50">
              {saving ? 'Creating...' : 'Create'}
            </button>
          </div>
        </div>
      </Modal>

      <Modal open={modal === 'edit'} onClose={() => setModal(null)} title="Update Maintenance Ticket">
        <div className="space-y-4">
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Status</label>
            <select value={editForm.status} onChange={(e) => setEditForm({ ...editForm, status: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent">
              {STATUSES.map((s) => <option key={s}>{s}</option>)}
            </select>
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Technician</label>
            <select value={editForm.technician} onChange={(e) => setEditForm({ ...editForm, technician: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent">
              <option value="">— Select Technician —</option>
              {users.filter((u) => ['technician', 'housekeeper'].includes(u.role?.toLowerCase())).map(u => <option key={u.id} value={u.displayName}>{u.displayName} ({u.role})</option>)}
            </select>
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Priority</label>
            <select value={editForm.priority} onChange={(e) => setEditForm({ ...editForm, priority: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent">
              {PRIORITIES.map((p) => <option key={p}>{p}</option>)}
            </select>
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <button onClick={() => setModal(null)} className="rounded-lg border border-primary/10 px-5 py-2.5 text-sm text-primary/60 hover:bg-primary/5">Cancel</button>
            <button onClick={handleEdit} disabled={saving}
              className="rounded-lg bg-accent px-5 py-2.5 text-sm text-white hover:bg-accent/90 disabled:opacity-50">
              {saving ? 'Saving...' : 'Save'}
            </button>
          </div>
        </div>
      </Modal>

      <ConfirmDeleteModal
        open={deleteTarget !== null}
        onClose={() => setDeleteTarget(null)}
        onConfirm={handleDelete}
        title="Delete Ticket"
        message={`Are you sure you want to delete ticket #${deleteTarget?.id} (${deleteTarget?.title})? This cannot be undone.`}
      />
    </div>
  );
}
