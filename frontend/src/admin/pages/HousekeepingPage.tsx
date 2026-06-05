import { useEffect, useState } from 'react';
import DataTable from '../components/DataTable';
import Modal, { ConfirmDeleteModal } from '../components/Modal';
import StatusBadge from '../components/StatusBadge';
import { getHotelApiBaseUrl, fetchRoomsOverview, fetchUsers } from '../../api';
import type { AuthUser, RoomOverview } from '../../types';

interface CleaningTask {
  id: number;
  roomNumber: string;
  status: string;
  assignedTo?: string;
  priority?: string;
}

const PRIORITIES = ['Low', 'Medium', 'High'];
const STATUSES = ['Dirty', 'Cleaning', 'Clean', 'Needs Inspection'];

interface CreateForm {
  roomNumber: string;
  assignedTo: string;
  priority: string;
}

interface EditForm {
  status: string;
  assignedTo: string;
}

const emptyCreateForm: CreateForm = { roomNumber: '', assignedTo: '', priority: 'Medium' };
const emptyEditForm: EditForm = { status: 'Dirty', assignedTo: '' };

export default function HousekeepingPage() {
  const [tasks, setTasks] = useState<CleaningTask[]>([]);
  const [users, setUsers] = useState<AuthUser[]>([]);
  const [rooms, setRooms] = useState<RoomOverview[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modal, setModal] = useState<'create' | 'edit' | null>(null);
  const [editingTask, setEditingTask] = useState<CleaningTask | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<CleaningTask | null>(null);
  const [createForm, setCreateForm] = useState<CreateForm>(emptyCreateForm);
  const [editForm, setEditForm] = useState<EditForm>(emptyEditForm);
  const [saving, setSaving] = useState(false);

  const apiBaseUrl = getHotelApiBaseUrl();

  async function loadTasks() {
    setLoading(true);
    setError(null);
    try {
      const [tasksRes, usersData, roomData] = await Promise.all([
        fetch(`${apiBaseUrl}/housekeeping/queue`),
        fetchUsers(),
        fetchRoomsOverview(),
      ]);
      if (!tasksRes.ok) throw new Error('Failed to fetch tasks');
      setUsers(usersData);
      setRooms(roomData);
      setTasks(await tasksRes.json());
    } catch {
      setError('Failed to load cleaning tasks. Is the backend running?');
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { loadTasks(); }, []);

  function openCreate() {
    setCreateForm(emptyCreateForm);
    setModal('create');
  }

  function openEdit(task: CleaningTask) {
    setEditingTask(task);
    setEditForm({
      status: task.status,
      assignedTo: task.assignedTo ?? '',
    });
    setModal('edit');
  }

  async function handleCreate() {
    setSaving(true);
    try {
      const res = await fetch(`${apiBaseUrl}/housekeeping/queue`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          roomNumber: createForm.roomNumber,
          assignedTo: createForm.assignedTo,
          priority: createForm.priority,
        }),
      });
      if (!res.ok) throw new Error('Failed to create');
      setModal(null);
      setCreateForm(emptyCreateForm);
      await loadTasks();
    } catch {
      setError('Failed to create cleaning task');
    } finally {
      setSaving(false);
    }
  }

  async function handleEdit() {
    if (!editingTask) return;
    setSaving(true);
    try {
      const res = await fetch(`${apiBaseUrl}/housekeeping/queue/${editingTask.id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          status: editForm.status,
          assignedTo: editForm.assignedTo,
        }),
      });
      if (!res.ok) throw new Error('Failed to update');
      setModal(null);
      setEditingTask(null);
      setEditForm(emptyEditForm);
      await loadTasks();
    } catch {
      setError('Failed to update cleaning task');
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete() {
    if (!deleteTarget) return;
    try {
      const res = await fetch(`${apiBaseUrl}/housekeeping/queue/${deleteTarget.id}`, {
        method: 'DELETE',
      });
      if (!res.ok) throw new Error('Failed to delete');
      setDeleteTarget(null);
      await loadTasks();
    } catch {
      setError('Failed to delete cleaning task');
    }
  }

  return (
    <div className="px-6 py-8">
      <div className="mb-8">
        <p className="text-xs uppercase tracking-[0.35em] text-accent">Operations</p>
        <h1 className="mt-2 text-2xl font-bold text-primary">Housekeeping</h1>
        <p className="mt-1 text-sm text-primary/50">Manage cleaning tasks and room statuses</p>
      </div>

      {error && (
        <div className="mb-4 rounded-xl border border-rose-200 bg-rose-50 p-3 text-sm text-rose-700">{error}</div>
      )}

      <DataTable
        columns={[
          { key: 'id', label: 'ID', sortable: true },
          { key: 'roomNumber', label: 'Room', sortable: true, render: (t: CleaningTask) => <span className="font-semibold">#{t.roomNumber}</span> },
          { key: 'status', label: 'Status', sortable: true, render: (t: CleaningTask) => <StatusBadge status={t.status} /> },
          { key: 'priority', label: 'Priority', sortable: true, render: (t: CleaningTask) => <StatusBadge status={t.priority ?? 'Medium'} /> },
          { key: 'assignedTo', label: 'Assigned To', sortable: true, render: (t: CleaningTask) => t.assignedTo || <span className="text-primary/30">—</span> },
        ]}
        data={tasks}
        keyExtractor={(t) => t.id}
        searchKeys={['roomNumber', 'status', 'assignedTo', 'priority']}
        searchPlaceholder="Search tasks..."
        loading={loading}
        toolbar={
          <button
            onClick={openCreate}
            className="rounded-lg bg-accent px-5 py-2.5 text-sm font-medium text-white hover:bg-accent/90"
          >
            + Add Task
          </button>
        }
        actions={(row) => (
          <>
            <button onClick={() => openEdit(row)} className="rounded-lg border border-primary/10 px-3 py-1.5 text-xs text-primary/60 hover:bg-primary/5">Edit</button>
            <button onClick={() => setDeleteTarget(row)} className="rounded-lg border border-rose-200 px-3 py-1.5 text-xs text-rose-600 hover:bg-rose-50">Delete</button>
          </>
        )}
      />

      <Modal open={modal === 'create'} onClose={() => setModal(null)} title="Add Cleaning Task">
        <div className="space-y-4">
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Room Number</label>
            <select
              value={createForm.roomNumber}
              onChange={(e) => setCreateForm({ ...createForm, roomNumber: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent"
            >
              <option value="">Select room</option>
              {rooms.map((room) => (
                <option key={room.id} value={room.roomNumber}>#{room.roomNumber} ({room.status})</option>
              ))}
            </select>
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Assigned To</label>
            <select
              value={createForm.assignedTo}
              onChange={(e) => setCreateForm({ ...createForm, assignedTo: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent"
            >
              <option value="">— Select Staff —</option>
              {users.filter(u => u.role === 'Housekeeper').map(u => <option key={u.id} value={u.displayName}>{u.displayName} ({u.role})</option>)}
            </select>
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Priority</label>
            <select
              value={createForm.priority}
              onChange={(e) => setCreateForm({ ...createForm, priority: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent"
            >
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

      <Modal open={modal === 'edit'} onClose={() => { setModal(null); setEditingTask(null); }} title={editingTask ? `Edit Task — Room ${editingTask.roomNumber}` : 'Edit Task'}>
        <div className="space-y-4">
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Status</label>
            <select
              value={editForm.status}
              onChange={(e) => setEditForm({ ...editForm, status: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent"
            >
              {STATUSES.map((s) => <option key={s}>{s}</option>)}
            </select>
          </div>
          <div>
            <label className="mb-1 block text-xs uppercase tracking-wider text-primary/50">Assigned To</label>
            <select
              value={editForm.assignedTo}
              onChange={(e) => setEditForm({ ...editForm, assignedTo: e.target.value })}
              className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent"
            >
              <option value="">— Select Staff —</option>
              {users.filter(u => u.role === 'Housekeeper').map(u => <option key={u.id} value={u.displayName}>{u.displayName} ({u.role})</option>)}
            </select>
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <button onClick={() => { setModal(null); setEditingTask(null); }} className="rounded-lg border border-primary/10 px-5 py-2.5 text-sm text-primary/60 hover:bg-primary/5">Cancel</button>
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
        title="Delete Task"
        message={`Are you sure you want to delete the cleaning task for room #${deleteTarget?.roomNumber}? This action cannot be undone.`}
      />
    </div>
  );
}
