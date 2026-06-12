import { useEffect, useState } from 'react';
import { DataTable } from '../components';
import type { Column } from '../components/DataTable';
import Modal, { ConfirmDeleteModal } from '../components/Modal';
import StatusBadge from '../components/StatusBadge';
import { getHotelApiBaseUrl, getAuthHeaders, guestCheckIn, guestCheckOut, fetchBookingsAdmin } from '../../api';
import type { BookingRecord } from '../../types';

function getAccessToken(): string {
  const store = (window as unknown as Record<string, unknown>).__hotelos_auth_store__ as
    | { getState(): { accessToken?: string } }
    | undefined;
  return store?.getState().accessToken ?? '';
}

const BOOKING_STATUSES = ['Booked', 'CheckedIn', 'CheckedOut', 'Cancelled'];

interface EditForm {
  guestName: string;
  roomNumber: string;
  checkInDate: string;
  checkOutDate: string;
  status: string;
}

export default function Bookings() {
  const [bookings, setBookings] = useState<BookingRecord[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const [rooms, setRooms] = useState<{ id: number; roomNumber: string; status: string }[]>([]);
  const [showCheckIn, setShowCheckIn] = useState(false);
  const [checkInForm, setCheckInForm] = useState({
    guestName: '',
    email: '',
    adults: 1,
    kids: 0,
    checkInDate: '',
    checkOutDate: '',
    roomId: 0,
  });
  const [saving, setSaving] = useState(false);

  const [showCheckOut, setShowCheckOut] = useState<BookingRecord | null>(null);

  const [editTarget, setEditTarget] = useState<BookingRecord | null>(null);
  const [editForm, setEditForm] = useState<EditForm>({
    guestName: '',
    roomNumber: '',
    checkInDate: '',
    checkOutDate: '',
    status: '',
  });

  const [deleteTarget, setDeleteTarget] = useState<BookingRecord | null>(null);

  const baseUrl = getHotelApiBaseUrl();

  function showSuccess(msg: string) {
    setSuccess(msg);
    setTimeout(() => setSuccess(null), 4000);
  }

  async function load() {
    setLoading(true);
    setError(null);
    try {
      const data = await fetchBookingsAdmin();
      setBookings(data);
    } catch {
      setError('Failed to load bookings. Is the backend running?');
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { load(); }, []);

  useEffect(() => {
    (async () => {
      try {
        const res = await fetch(`${baseUrl}/room/rooms`, {
          headers: { ...getAuthHeaders() },
        });
        if (res.ok) {
          const data: { id: number; roomNumber: string; status: string }[] = await res.json();
          setRooms(data.filter((r) => r.status === 'Available'));
        }
      } catch { /* ignore */ }
    })();
  }, []);

  async function handleCheckIn() {
    setSaving(true);
    setError(null);
    try {
      await guestCheckIn({
        guestName: checkInForm.guestName,
        email: checkInForm.email,
        adults: checkInForm.adults,
        kids: checkInForm.kids,
        checkInDate: checkInForm.checkInDate,
        checkOutDate: checkInForm.checkOutDate,
        roomId: checkInForm.roomId || undefined,
      });
      showSuccess('Guest checked in successfully. Booking created, event published, notification sent.');
      setShowCheckIn(false);
      setCheckInForm({ guestName: '', email: '', adults: 1, kids: 0, checkInDate: '', checkOutDate: '', roomId: 0 });
      await load();
    } catch {
      setError('Check-in failed. Is the backend running?');
    } finally {
      setSaving(false);
    }
  }

  async function handleCheckOut(target: BookingRecord) {
    setError(null);
    try {
      await guestCheckOut({ bookingId: target.id, notes: '' });
      showSuccess(`Checked out Booking #${target.id}. Event published.`);
      setShowCheckOut(null);
      await load();
    } catch {
      setError('Check-out failed.');
    }
  }

  function openEdit(target: BookingRecord) {
    setEditTarget(target);
    setEditForm({
      guestName: target.guestName,
      roomNumber: target.roomNumber,
      checkInDate: target.checkInDate,
      checkOutDate: target.checkOutDate,
      status: target.status,
    });
  }

  async function handleEdit() {
    if (!editTarget) return;
    setSaving(true);
    setError(null);
    try {
      const token = getAccessToken();
      const response = await fetch(`${baseUrl}/reception/bookings/${editTarget.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          ...(token ? { Authorization: `Bearer ${token}` } : {}),
        },
        body: JSON.stringify(editForm),
      });
      if (!response.ok) throw new Error('Failed to update booking');
      showSuccess('Booking updated successfully.');
      setEditTarget(null);
      await load();
    } catch {
      setError('Failed to update booking.');
    } finally {
      setSaving(false);
    }
  }

  function handlePrint(target: BookingRecord) {
    const win = window.open('', '_blank');
    if (!win) return;
    win.document.write(`
      <html><head><title>Booking #${target.id}</title>
      <style>
        body { font-family: 'Courier New', monospace; padding: 40px; color: #333; }
        h1 { font-size: 24px; margin-bottom: 5px; }
        .header { border-bottom: 2px solid #333; padding-bottom: 15px; margin-bottom: 20px; }
        .hotel { font-size: 14px; color: #666; }
        .row { display: flex; justify-content: space-between; padding: 8px 0; border-bottom: 1px dashed #ddd; }
        .label { font-weight: bold; color: #555; }
        .footer { margin-top: 30px; font-size: 12px; color: #999; text-align: center; }
      </style></head><body>
      <div class="header">
        <h1>HotelOS</h1>
        <div class="hotel">Booking Confirmation #${target.id}</div>
      </div>
      <div class="row"><span class="label">Guest</span><span>${target.guestName}</span></div>
      <div class="row"><span class="label">Room</span><span>${target.roomNumber}</span></div>
      <div class="row"><span class="label">Status</span><span>${target.status}</span></div>
      <div class="row"><span class="label">Check-In</span><span>${target.checkInDate}</span></div>
      <div class="row"><span class="label">Check-Out</span><span>${target.checkOutDate}</span></div>
      <div class="row"><span class="label">Total</span><span>$${target.total}</span></div>
      <div class="footer">Printed on ${new Date().toLocaleString()}</div>
      <script>window.print();window.close();<\\/script>
    </body></html>`);
    win.document.close();
  }

  async function handleCancel(target: BookingRecord) {
    if (!window.confirm(`Cancel booking #${target.id} for ${target.guestName}? This will refund 50% cash.`)) {
      return;
    }

    setError(null);
    try {
      const token = getAccessToken();
      const response = await fetch(`${baseUrl}/reception/bookings/${target.id}/cancel`, {
        method: 'POST',
        headers: token ? { Authorization: `Bearer ${token}` } : {},
      });
      if (!response.ok) throw new Error('Failed to cancel booking');
      showSuccess(`Booking #${target.id} cancelled and refunded.`);
      await load();
    } catch {
      setError('Failed to cancel booking.');
    }
  }

  async function handleDelete() {
    if (!deleteTarget) return;
    setError(null);
    try {
      const token = getAccessToken();
      const response = await fetch(`${baseUrl}/reception/bookings/${deleteTarget.id}`, {
        method: 'DELETE',
        headers: token ? { Authorization: `Bearer ${token}` } : {},
      });
      if (!response.ok) throw new Error('Failed to delete booking');
      showSuccess('Booking deleted successfully.');
      setDeleteTarget(null);
      await load();
    } catch {
      setError('Failed to delete booking.');
    }
  }

  const columns: Column<BookingRecord>[] = [
    { key: 'guestName', label: 'Guest Name', sortable: true },
    {
      key: 'roomNumber',
      label: 'Room Number',
      sortable: true,
      render: (b: BookingRecord) => <span className="font-semibold">#{b.roomNumber}</span>,
    },
    {
      key: 'status',
      label: 'Status',
      sortable: true,
      render: (b: BookingRecord) => <StatusBadge status={b.status} />,
    },
    { key: 'checkInDate', label: 'Check In', sortable: true },
    { key: 'checkOutDate', label: 'Check Out', sortable: true },
    {
      key: 'total',
      label: 'Total Price',
      sortable: true,
      render: (b: BookingRecord) => <span className="font-medium">${b.total?.toLocaleString() ?? 0}</span>,
    },
  ];

  return (
    <div className="px-6 py-8">
      <div className="mb-8 flex items-center justify-between">
        <div>
          <p className="text-xs uppercase tracking-[0.35em] text-accent">Reservation Flow</p>
          <h1 className="mt-2 text-2xl font-bold text-primary">Bookings</h1>
          <p className="mt-1 text-sm text-primary/50">
            Full reservation: validate → assign room → book → event → notify
          </p>
        </div>
        <button
          onClick={() => setShowCheckIn(!showCheckIn)}
          className="rounded-lg bg-accent px-5 py-2.5 text-sm font-medium text-white hover:bg-accent/90"
        >
          + Check In
        </button>
      </div>

      {error && (
        <div className="mb-4 rounded-xl border border-rose-200 bg-rose-50 p-3 text-sm text-rose-700">
          {error}
        </div>
      )}

      {success && (
        <div className="mb-4 rounded-xl border border-emerald-200 bg-emerald-50 p-3 text-sm text-emerald-700">
          {success}
        </div>
      )}

      {showCheckIn && (
        <div className="mb-6 rounded-2xl border border-primary/10 bg-white p-6">
          <h2 className="mb-4 text-lg font-semibold">Guest Check-In (Reservation Flow)</h2>
          <div className="grid gap-4 md:grid-cols-2">
            <div>
              <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Full Name</label>
              <input
                value={checkInForm.guestName}
                onChange={(e) => setCheckInForm({ ...checkInForm, guestName: e.target.value })}
                className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent"
              />
            </div>
            <div>
              <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Email</label>
              <input
                value={checkInForm.email}
                onChange={(e) => setCheckInForm({ ...checkInForm, email: e.target.value })}
                className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent"
              />
            </div>
            <div>
              <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Adults</label>
              <input
                type="number"
                min={1}
                value={checkInForm.adults}
                onChange={(e) => setCheckInForm({ ...checkInForm, adults: +e.target.value })}
                className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent"
              />
            </div>
            <div>
              <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Kids</label>
              <input
                type="number"
                min={0}
                value={checkInForm.kids}
                onChange={(e) => setCheckInForm({ ...checkInForm, kids: +e.target.value })}
                className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent"
              />
            </div>
            <div>
              <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Room</label>
              <select
                value={checkInForm.roomId}
                onChange={(e) => setCheckInForm({ ...checkInForm, roomId: +e.target.value })}
                className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent"
              >
                <option value={0}>Auto-assign</option>
                {rooms.map((r) => (
                  <option key={r.id} value={r.id}>#{r.roomNumber} ({r.status})</option>
                ))}
              </select>
            </div>
            <div>
              <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Check-In Date</label>
              <input
                type="date"
                min={new Date().toISOString().split('T')[0]}
                value={checkInForm.checkInDate}
                onChange={(e) => setCheckInForm({ ...checkInForm, checkInDate: e.target.value })}
                className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent"
              />
            </div>
            <div>
              <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Check-Out Date</label>
              <input
                type="date"
                min={checkInForm.checkInDate || new Date().toISOString().split('T')[0]}
                value={checkInForm.checkOutDate}
                onChange={(e) => setCheckInForm({ ...checkInForm, checkOutDate: e.target.value })}
                className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent"
              />
            </div>
          </div>
          <div className="mt-4 flex gap-3">
            <button
              onClick={handleCheckIn}
              disabled={saving}
              className="rounded-lg bg-accent px-5 py-2.5 text-sm text-white hover:bg-accent/90 disabled:opacity-50"
            >
              {saving ? 'Processing...' : 'Check In'}
            </button>
            <button
              onClick={() => setShowCheckIn(false)}
              className="rounded-lg border border-primary/10 px-5 py-2.5 text-sm text-primary/60 hover:bg-primary/5"
            >
              Cancel
            </button>
          </div>
        </div>
      )}

      <DataTable
        columns={columns}
        data={bookings}
        keyExtractor={(b) => b.id}
        searchKeys={['guestName', 'roomNumber', 'status']}
        searchPlaceholder="Search bookings..."
        loading={loading}
        actions={(row) => (
          <>
            {row.status !== 'CheckedOut' && row.status !== 'Cancelled' && (
              <button
                onClick={() => setShowCheckOut(row)}
                className="rounded-lg border border-rose-200 px-3 py-1.5 text-xs text-rose-600 hover:bg-rose-50"
              >
                Check Out
              </button>
            )}
            {row.status !== 'CheckedOut' && row.status !== 'Cancelled' && (
              <button
                onClick={() => handleCancel(row)}
                className="rounded-lg border border-amber-200 px-3 py-1.5 text-xs text-amber-600 hover:bg-amber-50"
              >
                Cancel
              </button>
            )}
            <button
              onClick={() => handlePrint(row)}
              className="rounded-lg border border-primary/10 px-3 py-1.5 text-xs text-primary/60 hover:bg-primary/5"
            >
              Print
            </button>
            <button
              onClick={() => openEdit(row)}
              className="rounded-lg border border-primary/10 px-3 py-1.5 text-xs text-primary/60 hover:bg-primary/5"
            >
              Edit
            </button>
            <button
              onClick={() => setDeleteTarget(row)}
              className="rounded-lg border border-rose-200 px-3 py-1.5 text-xs text-rose-600 hover:bg-rose-50"
            >
              Delete
            </button>
          </>
        )}
      />

      <Modal
        open={editTarget !== null}
        onClose={() => setEditTarget(null)}
        title={`Edit Booking #${editTarget?.id ?? ''}`}
        size="md"
      >
        <div className="space-y-4">
          {editTarget && (
            <div className="rounded-lg border border-primary/10 bg-primary/[0.02] p-4">
              <p className="text-sm font-medium text-primary">
                {editTarget.guestName} — Room #{editTarget.roomNumber}
              </p>
              <p className="mt-1 text-xs text-primary/40">Originally: {editTarget.checkInDate} → {editTarget.checkOutDate}</p>
            </div>
          )}
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Guest Name</label>
            <input
              value={editForm.guestName}
              onChange={(e) => setEditForm({ ...editForm, guestName: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent"
            />
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Room Number</label>
            <input
              value={editForm.roomNumber}
              onChange={(e) => setEditForm({ ...editForm, roomNumber: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent"
            />
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Check-In Date</label>
              <input
                type="date"
                value={editForm.checkInDate}
                onChange={(e) => setEditForm({ ...editForm, checkInDate: e.target.value })}
                className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent"
              />
            </div>
            <div>
              <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Check-Out Date</label>
              <input
                type="date"
                value={editForm.checkOutDate}
                onChange={(e) => setEditForm({ ...editForm, checkOutDate: e.target.value })}
                className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent"
              />
            </div>
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Status</label>
            <select
              value={editForm.status}
              onChange={(e) => setEditForm({ ...editForm, status: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent"
            >
              {BOOKING_STATUSES.map((s) => (
                <option key={s} value={s}>
                  {s}
                </option>
              ))}
            </select>
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <button
              onClick={() => setEditTarget(null)}
              className="rounded-lg border border-primary/10 px-5 py-2.5 text-sm text-primary/60 hover:bg-primary/5"
            >
              Cancel
            </button>
            <button
              onClick={handleEdit}
              disabled={saving}
              className="rounded-lg bg-accent px-5 py-2.5 text-sm text-white hover:bg-accent/90 disabled:opacity-50"
            >
              {saving ? 'Saving...' : 'Save'}
            </button>
          </div>
        </div>
      </Modal>

      <ConfirmDeleteModal
        open={deleteTarget !== null}
        onClose={() => setDeleteTarget(null)}
        onConfirm={handleDelete}
        title="Delete Booking"
        message={`Are you sure you want to delete booking #${deleteTarget?.id} for ${deleteTarget?.guestName} (Room #${deleteTarget?.roomNumber})? This action cannot be undone.`}
      />

      {showCheckOut && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/30">
          <div className="w-full max-w-sm rounded-2xl bg-white p-6 shadow-xl">
            <h3 className="text-lg font-semibold">Confirm Check-Out</h3>
            <p className="mt-2 text-sm text-primary/60">
              Check out {showCheckOut.guestName} from Room #{showCheckOut.roomNumber}?
            </p>
            <div className="mt-6 flex gap-3 justify-end">
              <button
                onClick={() => setShowCheckOut(null)}
                className="rounded-lg border border-primary/10 px-4 py-2 text-sm"
              >
                Cancel
              </button>
              <button
                onClick={() => handleCheckOut(showCheckOut)}
                className="rounded-lg bg-rose-600 px-4 py-2 text-sm text-white"
              >
                Check Out
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
