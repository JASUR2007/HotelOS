import { useEffect, useState } from 'react';
import DataTable from '../components/DataTable';
import Modal, { ConfirmDeleteModal } from '../components/Modal';
import { getHotelApiBaseUrl } from '../../api';
import type { GuestRecord } from '../../types';

interface GuestForm {
  fullName: string;
  email: string;
  phone: string;
  notes: string;
}

const emptyForm: GuestForm = { fullName: '', email: '', phone: '', notes: '' };

function getAuthHeaders(): Record<string, string> {
  const store = (window as unknown as Record<string, unknown>).__hotelos_auth_store__ as
    | { getState(): { accessToken?: string } }
    | undefined;
  const token = store?.getState().accessToken;
  return token ? { Authorization: `Bearer ${token}` } : {};
}

async function fetchGuestsList(): Promise<GuestRecord[]> {
  const url = `${getHotelApiBaseUrl()}/reception/guests`;
  const res = await fetch(url, {
    headers: { 'Content-Type': 'application/json', ...getAuthHeaders() },
  });
  if (!res.ok) throw new Error('Failed to fetch guests');
  return res.json();
}

async function createGuestApi(data: { fullName: string; email: string }): Promise<{ guestId: number; fullName: string; email: string }> {
  const url = `${getHotelApiBaseUrl()}/reception/guests`;
  const res = await fetch(url, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', ...getAuthHeaders() },
    body: JSON.stringify(data),
  });
  if (!res.ok) throw new Error('Failed to create guest');
  return res.json();
}

async function updateGuestApi(id: number, data: GuestForm): Promise<GuestRecord> {
  const url = `${getHotelApiBaseUrl()}/reception/guests/${id}`;
  const res = await fetch(url, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json', ...getAuthHeaders() },
    body: JSON.stringify(data),
  });
  if (!res.ok) throw new Error('Failed to update guest');
  return res.json();
}

async function deleteGuestApi(id: number): Promise<void> {
  const url = `${getHotelApiBaseUrl()}/reception/guests/${id}`;
  const res = await fetch(url, {
    method: 'DELETE',
    headers: { 'Content-Type': 'application/json', ...getAuthHeaders() },
  });
  if (!res.ok) throw new Error('Failed to delete guest');
}

export default function Guests() {
  const [guests, setGuests] = useState<GuestRecord[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState<GuestForm>(emptyForm);
  const [editTarget, setEditTarget] = useState<GuestRecord | null>(null);
  const [editForm, setEditForm] = useState<GuestForm>(emptyForm);
  const [deleteTarget, setDeleteTarget] = useState<GuestRecord | null>(null);
  const [saving, setSaving] = useState(false);

  async function load() {
    setLoading(true);
    setError(null);
    try {
      setGuests(await fetchGuestsList());
    } catch {
      setError('Failed to load guests.');
      setGuests([]);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { load(); }, []);

  async function handleCreate() {
    setSaving(true);
    try {
      await createGuestApi({ fullName: form.fullName, email: form.email });
      setMessage('Guest created successfully.');
      setTimeout(() => setMessage(''), 3000);
      setShowForm(false);
      setForm(emptyForm);
      await load();
    } catch {
      setError('Failed to create guest.');
    } finally {
      setSaving(false);
    }
  }

  function openEdit(guest: GuestRecord) {
    setEditForm({ fullName: guest.name, email: (guest as any).email || '', phone: '', notes: '' });
    setEditTarget(guest);
  }

  async function handleUpdate() {
    if (!editTarget) return;
    setSaving(true);
    try {
      await updateGuestApi(editTarget.id, editForm);
      setMessage('Guest updated successfully.');
      setEditTarget(null);
      setEditForm(emptyForm);
      await load();
    } catch {
      setError('Failed to update guest.');
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete() {
    if (!deleteTarget) return;
    setSaving(true);
    try {
      await deleteGuestApi(deleteTarget.id);
      setMessage('Guest deleted successfully.');
      setDeleteTarget(null);
      await load();
    } catch {
      setError('Failed to delete guest.');
    } finally {
      setSaving(false);
    }
  }

  function formatDate(dateStr: string) {
    if (!dateStr) return '-';
    try {
      return new Date(dateStr).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
    } catch {
      return dateStr;
    }
  }

  return (
    <div className="px-6 py-8">
      <div className="mb-8">
        <p className="text-xs uppercase tracking-[0.35em] text-accent">Reception</p>
        <h1 className="mt-2 text-2xl font-bold text-primary">Guest Registry</h1>
      </div>

      {message && (
        <div className="mb-4 rounded-xl border border-blue-200 bg-blue-50 p-3 text-sm text-blue-700">{message}</div>
      )}

      {error && (
        <div className="mb-4 rounded-xl border border-rose-200 bg-rose-50 p-3 text-sm text-rose-700">{error}</div>
      )}

      {showForm && (
        <div className="mb-6 rounded-2xl border border-primary/10 bg-white p-6">
          <h2 className="mb-4 text-lg font-semibold">New Guest</h2>
          <div className="grid gap-4 md:grid-cols-2">
            <div>
              <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Full Name</label>
              <input value={form.fullName} onChange={(e) => setForm({ ...form, fullName: e.target.value })}
                className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
            </div>
            <div>
              <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Email</label>
              <input type="email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })}
                className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
            </div>
            <div>
              <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Phone</label>
              <input type="text" value={form.phone} onChange={(e) => setForm({ ...form, phone: e.target.value })}
                className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
            </div>
            <div>
              <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Notes</label>
              <input value={form.notes} onChange={(e) => setForm({ ...form, notes: e.target.value })}
                className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
            </div>
          </div>
          <div className="mt-4 flex gap-3">
            <button onClick={handleCreate} disabled={saving}
              className="rounded-lg bg-accent px-5 py-2.5 text-sm text-white hover:bg-accent/90 disabled:opacity-50">
              {saving ? 'Creating...' : 'Create Guest'}
            </button>
            <button onClick={() => { setShowForm(false); setForm(emptyForm); }}
              className="rounded-lg border border-primary/10 px-5 py-2.5 text-sm text-primary/60 hover:bg-primary/5">
              Cancel
            </button>
          </div>
        </div>
      )}

      <DataTable
        columns={[
          { key: 'name', label: 'Name', sortable: true, render: (r: GuestRecord) => <span className="font-semibold">{r.name}</span> },
      { key: 'email', label: 'Email', sortable: true, render: (r: GuestRecord) => <span className="text-primary/50">{(r as any).email || '—'}</span> },
      { key: 'roomNumber', label: 'Room Number', sortable: true, render: (r: GuestRecord) => <span className="font-semibold">{r.roomNumber}</span> },
      { key: 'checkIn', label: 'Check-In', sortable: true, render: (r: GuestRecord) => formatDate(r.checkIn) },
      { key: 'checkOut', label: 'Check-Out', sortable: true, render: (r: GuestRecord) => formatDate(r.checkOut) },
      { key: 'balance', label: 'Balance', sortable: true, render: (r: GuestRecord) => <span className="font-medium">${r.balance?.toLocaleString?.() ?? r.balance ?? 0}</span> },
        ]}
        data={guests}
        keyExtractor={(r) => r.id}
        searchKeys={['name', 'roomNumber']}
        searchPlaceholder="Search guests..."
        loading={loading}
        toolbar={
          <button
            onClick={() => { setShowForm(!showForm); setMessage(''); setError(null); }}
            className="rounded-lg bg-accent px-5 py-2.5 text-sm font-medium text-white hover:bg-accent/90"
          >
            + Add Guest
          </button>
        }
        actions={(row) => (
          <>
            <button onClick={() => openEdit(row)} className="rounded-lg border border-primary/10 px-3 py-1.5 text-xs text-primary/60 hover:bg-primary/5">Edit</button>
            <button onClick={() => setDeleteTarget(row)} className="rounded-lg border border-rose-200 px-3 py-1.5 text-xs text-rose-600 hover:bg-rose-50">Delete</button>
          </>
        )}
      />

      <Modal open={editTarget !== null} onClose={() => { setEditTarget(null); setEditForm(emptyForm); }} title="Edit Guest">
        <div className="space-y-4">
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Full Name</label>
            <input value={editForm.fullName} onChange={(e) => setEditForm({ ...editForm, fullName: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Email</label>
            <input type="email" value={editForm.email} onChange={(e) => setEditForm({ ...editForm, email: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Phone</label>
            <input type="text" value={editForm.phone} onChange={(e) => setEditForm({ ...editForm, phone: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Notes</label>
            <input value={editForm.notes} onChange={(e) => setEditForm({ ...editForm, notes: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <button onClick={() => { setEditTarget(null); setEditForm(emptyForm); }}
              className="rounded-lg border border-primary/10 px-5 py-2.5 text-sm text-primary/60 hover:bg-primary/5">
              Cancel
            </button>
            <button onClick={handleUpdate} disabled={saving}
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
        title="Delete Guest"
        message={`Are you sure you want to delete guest "${deleteTarget?.name}" (Room #${deleteTarget?.roomNumber})? This action cannot be undone.`}
      />
    </div>
  );
}
