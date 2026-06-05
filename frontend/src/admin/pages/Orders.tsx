import { useEffect, useState } from 'react';
import DataTable from '../components/DataTable';
import Modal, { ConfirmDeleteModal } from '../components/Modal';
import StatusBadge from '../components/StatusBadge';
import { fetchGuests, fetchRoomsOverview, getHotelApiBaseUrl } from '../../api';
import type { GuestRecord, OrderItem, RoomOverview } from '../../types';

interface MenuItemRecord { id: number; name: string; price: number; }

const ORDER_STATUSES = ['Preparing', 'Ready', 'Out For Delivery', 'Delivered', 'Cancelled'];

interface OrderForm {
  roomNumber: string;
  guestName: string;
  items: string[];
}

const emptyForm: OrderForm = { roomNumber: '', guestName: '', items: [] };

export default function Orders() {
  const [orders, setOrders] = useState<OrderItem[]>([]);
  const [menuItems, setMenuItems] = useState<MenuItemRecord[]>([]);
  const [guests, setGuests] = useState<GuestRecord[]>([]);
  const [rooms, setRooms] = useState<RoomOverview[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [modal, setModal] = useState<'create' | 'edit' | null>(null);
  const [menuModal, setMenuModal] = useState(false);
  const [editingOrder, setEditingOrder] = useState<OrderItem | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<OrderItem | null>(null);
  const [form, setForm] = useState<OrderForm>(emptyForm);
  const [editStatus, setEditStatus] = useState('');
  const [saving, setSaving] = useState(false);

  const [newMenuItem, setNewMenuItem] = useState({ name: '', price: 0 });

  const baseUrl = getHotelApiBaseUrl();

  async function loadOrders() {
    setLoading(true);
    setError(null);
    try {
      const response = await fetch(`${baseUrl}/room/orders`);
      if (!response.ok) throw new Error('Failed to fetch orders');
      const data: OrderItem[] = await response.json();
      setOrders(data);
    } catch {
      setError('Failed to load orders. Is the backend running?');
    } finally {
      setLoading(false);
    }
  }

  async function loadMenuItems() {
    try {
      const r = await fetch(`${baseUrl}/room/menuitems`);
      if (r.ok) setMenuItems(await r.json());
    } catch { /* ignore */ }
  }

  async function loadOptions() {
    const [guestData, roomData] = await Promise.all([
      fetchGuests().catch(() => []),
      fetchRoomsOverview().catch(() => []),
    ]);
    setGuests(guestData);
    setRooms(roomData);
  }

  async function addMenuItem() {
    if (!newMenuItem.name || newMenuItem.price <= 0) return;
    try {
      await fetch(`${baseUrl}/room/menuitems`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(newMenuItem),
      });
      setNewMenuItem({ name: '', price: 0 });
      await loadMenuItems();
    } catch { /* ignore */ }
  }

  async function deleteMenuItem(id: number) {
    try {
      await fetch(`${baseUrl}/room/menuitems/${id}`, { method: 'DELETE' });
      await loadMenuItems();
    } catch { /* ignore */ }
  }

  useEffect(() => { loadOrders(); loadMenuItems(); loadOptions(); }, []);

  function showSuccess(msg: string) {
    setSuccess(msg);
    setTimeout(() => setSuccess(null), 3000);
  }

  function openCreate() {
    setForm(emptyForm);
    setEditingOrder(null);
    loadOptions().catch(() => undefined);
    setModal('create');
  }

  function openEdit(order: OrderItem) {
    setEditingOrder(order);
    setEditStatus(order.status);
    setModal('edit');
  }

  function toggleItem(item: string) {
    setForm((prev) => ({
      ...prev,
      items: prev.items.includes(item)
        ? prev.items.filter((i) => i !== item)
        : [...prev.items, item],
    }));
  }

  async function handleSave() {
    if (!form.roomNumber.trim() || !form.guestName.trim() || form.items.length === 0) {
      setError('Please fill in all fields and select at least one item.');
      return;
    }
    setSaving(true);
    setError(null);
    try {
      const response = await fetch(`${baseUrl}/room/orders`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          roomNumber: form.roomNumber,
          guestName: form.guestName,
          items: form.items,
        }),
      });
      if (!response.ok) throw new Error('Failed to create order');
      setModal(null);
      setForm(emptyForm);
      showSuccess('Order created successfully');
      await loadOrders();
    } catch {
      setError('Failed to create order');
    } finally {
      setSaving(false);
    }
  }

  async function handleStatusUpdate() {
    if (!editingOrder) return;
    setSaving(true);
    setError(null);
    try {
      const response = await fetch(`${baseUrl}/room/orders/${editingOrder.id}/status`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ status: editStatus }),
      });
      if (!response.ok) throw new Error('Failed to update status');
      setModal(null);
      setEditingOrder(null);
      setEditStatus('');
      showSuccess('Status updated successfully');
      await loadOrders();
    } catch {
      setError('Failed to update status');
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete() {
    if (!deleteTarget) return;
    setError(null);
    try {
      const response = await fetch(`${baseUrl}/room/orders/${deleteTarget.id}`, {
        method: 'DELETE',
      });
      if (!response.ok) throw new Error('Failed to delete order');
      setDeleteTarget(null);
      showSuccess('Order deleted successfully');
      await loadOrders();
    } catch {
      setError('Failed to delete order');
    }
  }

  return (
    <section className="px-6 py-8">
      <div className="mb-8">
        <p className="text-xs uppercase tracking-[0.35em] text-accent">Room service</p>
        <h1 className="mt-3 text-2xl font-bold text-primary">Food orders</h1>
      </div>

      {error && (
        <div className="mb-4 rounded-xl border border-rose-200 bg-rose-50 p-3 text-sm text-rose-700">{error}</div>
      )}

      {success && (
        <div className="mb-4 rounded-xl border border-emerald-200 bg-emerald-50 p-3 text-sm text-emerald-700">{success}</div>
      )}

      <DataTable
        columns={[
          { key: 'id', label: 'ID', sortable: true },
          {
            key: 'roomNumber',
            label: 'Room',
            sortable: true,
            render: (o: OrderItem) => <span className="font-semibold">#{o.roomNumber}</span>,
          },
          { key: 'guestName', label: 'Guest', sortable: true },
          {
            key: 'items',
            label: 'Items',
            sortable: false,
            render: (o: OrderItem) => (
              <span className="text-primary/70">{o.items?.join(', ')}</span>
            ),
          },
          {
            key: 'status',
            label: 'Status',
            sortable: true,
            render: (o: OrderItem) => <StatusBadge status={o.status} />,
          },
          {
            key: 'total',
            label: 'Total',
            sortable: true,
            render: (o: OrderItem) => <span className="font-medium">${o.total?.toLocaleString?.() ?? o.total ?? 0}</span>,
          },
          { key: 'updatedAt', label: 'Updated', sortable: true },
        ]}
        data={orders}
        keyExtractor={(o) => o.id}
        searchKeys={['roomNumber', 'guestName', 'status']}
        searchPlaceholder="Search orders..."
        loading={loading}
        toolbar={
          <div className="flex gap-2">
            <button
              onClick={() => { loadMenuItems(); setMenuModal(true); }}
              className="rounded-lg border border-primary/10 px-5 py-2.5 text-sm font-medium text-primary/60 hover:bg-primary/5"
            >
              Manage Menu
            </button>
            <button
              onClick={openCreate}
              className="rounded-lg bg-accent px-5 py-2.5 text-sm font-medium text-white hover:bg-accent/90"
            >
              + New Order
            </button>
          </div>
        }
        actions={(row) => (
          <>
            <button
              onClick={() => openEdit(row)}
              className="rounded-lg border border-primary/10 px-3 py-1.5 text-xs text-primary/60 hover:bg-primary/5"
            >
              Status
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

      <Modal open={menuModal} onClose={() => setMenuModal(false)} title="Menu Items">
        <div className="space-y-4">
          <div className="max-h-60 overflow-y-auto space-y-2">
            {menuItems.map(item => (
              <div key={item.id} className="flex items-center justify-between rounded-lg border border-primary/10 px-4 py-2.5">
                <div>
                  <span className="text-sm font-medium">{item.name}</span>
                  <span className="ml-2 text-xs text-primary/50">${item.price.toFixed(2)}</span>
                </div>
                <button onClick={() => deleteMenuItem(item.id)} className="text-xs text-rose-600 hover:text-rose-800">Delete</button>
              </div>
            ))}
            {menuItems.length === 0 && <p className="text-sm text-primary/30 text-center py-4">No menu items yet</p>}
          </div>
          <div className="flex gap-3 items-end">
            <div className="flex-1">
              <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Name</label>
              <input value={newMenuItem.name} onChange={e => setNewMenuItem({ ...newMenuItem, name: e.target.value })}
                className="w-full rounded-lg border border-primary/10 px-3 py-2 text-sm outline-none focus:border-accent" />
            </div>
            <div className="w-24">
              <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Price</label>
              <input type="number" step="0.01" min="0" value={newMenuItem.price} onChange={e => setNewMenuItem({ ...newMenuItem, price: parseFloat(e.target.value) || 0 })}
                className="w-full rounded-lg border border-primary/10 px-3 py-2 text-sm outline-none focus:border-accent" />
            </div>
            <button onClick={addMenuItem} disabled={!newMenuItem.name || newMenuItem.price <= 0}
              className="rounded-lg bg-accent px-4 py-2 text-sm text-white hover:bg-accent/90 disabled:opacity-50">Add</button>
          </div>
        </div>
      </Modal>

      <Modal open={modal === 'create'} onClose={() => setModal(null)} title="New Food Order">
        <div className="space-y-4">
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Room Number</label>
            <select
              value={form.roomNumber}
              onChange={(e) => setForm({ ...form, roomNumber: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent"
            >
              <option value="">Select room</option>
              {rooms.map((room) => (
                <option key={room.id} value={room.roomNumber}>#{room.roomNumber} ({room.status})</option>
              ))}
            </select>
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Guest Name</label>
            <select
              value={form.guestName}
              onChange={(e) => {
                const guest = guests.find((item) => item.name === e.target.value);
                setForm({
                  ...form,
                  guestName: e.target.value,
                  roomNumber: guest?.roomNumber || form.roomNumber,
                });
              }}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent"
            >
              <option value="">Select guest</option>
              {guests.map((guest) => (
                <option key={guest.id} value={guest.name}>
                  {guest.name}{guest.roomNumber ? ` - Room #${guest.roomNumber}` : ''}
                </option>
              ))}
            </select>
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Menu Items</label>
            <div className="space-y-2">
              {menuItems.map((item) => (
                <label
                  key={item.id}
                  className={`flex cursor-pointer items-center gap-3 rounded-lg border px-4 py-2.5 text-sm transition ${
                    form.items.includes(item.name)
                      ? 'border-accent bg-accent/5 text-accent'
                      : 'border-primary/10 text-primary/60 hover:border-primary/20'
                  }`}
                >
                  <input
                    type="checkbox"
                    checked={form.items.includes(item.name)}
                    onChange={() => toggleItem(item.name)}
                    className="h-4 w-4 rounded border-primary/20 text-accent accent-accent"
                  />
                  <span className="flex-1">{item.name}</span>
                  <span className="text-xs text-primary/50">${item.price.toFixed(2)}</span>
                </label>
              ))}
            </div>
            {form.items.length > 0 && (
              <p className="mt-2 text-xs text-primary/40">{form.items.length} item(s) selected</p>
            )}
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <button
              onClick={() => setModal(null)}
              className="rounded-lg border border-primary/10 px-5 py-2.5 text-sm text-primary/60 hover:bg-primary/5"
            >
              Cancel
            </button>
            <button
              onClick={handleSave}
              disabled={saving}
              className="rounded-lg bg-accent px-5 py-2.5 text-sm text-white hover:bg-accent/90 disabled:opacity-50"
            >
              {saving ? 'Creating...' : 'Create Order'}
            </button>
          </div>
        </div>
      </Modal>

      <Modal open={modal === 'edit'} onClose={() => setModal(null)} title="Update Order Status">
        <div className="space-y-4">
          {editingOrder && (
            <div className="rounded-lg border border-primary/10 bg-primary/[0.02] p-4">
              <p className="text-sm font-medium text-primary">Order #{editingOrder.id}</p>
              <p className="mt-1 text-xs text-primary/50">
                Room #{editingOrder.roomNumber} — {editingOrder.guestName}
              </p>
              <p className="mt-1 text-xs text-primary/40">{editingOrder.items?.join(', ')}</p>
            </div>
          )}
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Status</label>
            <select
              value={editStatus}
              onChange={(e) => setEditStatus(e.target.value)}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent"
            >
              {ORDER_STATUSES.map((s) => (
                <option key={s} value={s}>
                  {s}
                </option>
              ))}
            </select>
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <button
              onClick={() => setModal(null)}
              className="rounded-lg border border-primary/10 px-5 py-2.5 text-sm text-primary/60 hover:bg-primary/5"
            >
              Cancel
            </button>
            <button
              onClick={handleStatusUpdate}
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
        title="Delete Order"
        message={`Are you sure you want to delete order #${deleteTarget?.id} for ${deleteTarget?.guestName} (Room #${deleteTarget?.roomNumber})? This action cannot be undone.`}
      />
    </section>
  );
}
