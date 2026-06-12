import { useEffect, useState } from 'react';
import { DataTable, Modal, ConfirmDeleteModal, StatusBadge } from '../components';
import type { Column } from '../components/DataTable';
import { fetchRoomKeys, fetchMasterKeys, issueRoomKey, returnRoomKey, markKeyLost, createMasterKey, issueMasterKey, returnMasterKey, deleteRoomKey, deleteMasterKey, fetchRooms } from '../../api';
import type { RoomKeyDto, MasterKeyDto, RoomDto } from '../../types';

type Tab = 'room' | 'master';

export default function RoomKeys() {
  const [tab, setTab] = useState<Tab>('room');
  const [keys, setKeys] = useState<RoomKeyDto[]>([]);
  const [masterKeys, setMasterKeys] = useState<MasterKeyDto[]>([]);
  const [rooms, setRooms] = useState<RoomDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [saving, setSaving] = useState(false);

  // Issue key modal
  const [showIssue, setShowIssue] = useState(false);
  const [issueRoomId, setIssueRoomId] = useState(0);
  const [issueRoomNumber, setIssueRoomNumber] = useState('');
  const [issueTo, setIssueTo] = useState('');
  const [issueBy, setIssueBy] = useState('');

  // Create master key modal
  const [showCreateMaster, setShowCreateMaster] = useState(false);
  const [mkName, setMkName] = useState('');
  const [mkDesc, setMkDesc] = useState('');
  const [mkScope, setMkScope] = useState('');

  // Issue master key modal
  const [showIssueMaster, setShowIssueMaster] = useState(false);
  const [issueMasterId, setIssueMasterId] = useState(0);
  const [issueMasterTo, setIssueMasterTo] = useState('');

  // Delete targets
  const [deleteKeyTarget, setDeleteKeyTarget] = useState<RoomKeyDto | null>(null);
  const [deleteMasterTarget, setDeleteMasterTarget] = useState<MasterKeyDto | null>(null);

  async function load() {
    setLoading(true);
    try {
      const [k, mk, r] = await Promise.all([fetchRoomKeys(), fetchMasterKeys(), fetchRooms()]);
      setKeys(k);
      setMasterKeys(mk);
      setRooms(r);
    } catch { setError('Failed to load data'); }
    finally { setLoading(false); }
  }

  useEffect(() => { load(); }, []);

  useEffect(() => { if (success) { const t = setTimeout(() => setSuccess(''), 4000); return () => clearTimeout(t); } }, [success]);

  async function handleIssueKey() {
    setSaving(true); setError('');
    try {
      await issueRoomKey({ roomId: issueRoomId, roomNumber: issueRoomNumber, issuedTo: issueTo, issuedBy: issueBy });
      setSuccess('Key issued');
      setShowIssue(false);
      setIssueTo(''); setIssueBy('');
      load();
    } catch (e: unknown) { setError(e instanceof Error ? e.message : 'Failed to issue key'); }
    finally { setSaving(false); }
  }

  async function handleReturnKey(id: number) {
    setSaving(true);
    try {
      await returnRoomKey(id);
      setSuccess('Key returned');
      load();
    } catch (e: unknown) { setError(e instanceof Error ? e.message : 'Failed to return key'); }
    finally { setSaving(false); }
  }

  async function handleMarkLost(id: number) {
    setSaving(true);
    try {
      await markKeyLost(id);
      setSuccess('Key marked as lost');
      load();
    } catch (e: unknown) { setError(e instanceof Error ? e.message : 'Failed'); }
    finally { setSaving(false); }
  }

  async function handleCreateMaster() {
    setSaving(true); setError('');
    try {
      await createMasterKey({ name: mkName, description: mkDesc, accessScope: mkScope });
      setSuccess('Master key created');
      setShowCreateMaster(false);
      setMkName(''); setMkDesc(''); setMkScope('');
      load();
    } catch (e: unknown) { setError(e instanceof Error ? e.message : 'Failed to create master key'); }
    finally { setSaving(false); }
  }

  async function handleIssueMaster() {
    setSaving(true); setError('');
    try {
      await issueMasterKey(issueMasterId, { issuedTo: issueMasterTo });
      setSuccess('Master key issued');
      setShowIssueMaster(false);
      setIssueMasterTo('');
      load();
    } catch (e: unknown) { setError(e instanceof Error ? e.message : 'Failed to issue master key'); }
    finally { setSaving(false); }
  }

  async function handleReturnMaster(id: number) {
    setSaving(true);
    try {
      await returnMasterKey(id);
      setSuccess('Master key returned');
      load();
    } catch (e: unknown) { setError(e instanceof Error ? e.message : 'Failed to return master key'); }
    finally { setSaving(false); }
  }

  async function handleDeleteKey() {
    if (!deleteKeyTarget) return;
    try {
      await deleteRoomKey(deleteKeyTarget.id);
      setSuccess('Key deleted');
      setDeleteKeyTarget(null);
      load();
    } catch (e: unknown) { setError(e instanceof Error ? e.message : 'Failed to delete'); }
  }

  async function handleDeleteMaster() {
    if (!deleteMasterTarget) return;
    try {
      await deleteMasterKey(deleteMasterTarget.id);
      setSuccess('Master key deleted');
      setDeleteMasterTarget(null);
      load();
    } catch (e: unknown) { setError(e instanceof Error ? e.message : 'Failed to delete'); }
  }

  const roomColumns: Column<RoomKeyDto>[] = [
    { key: 'roomNumber', label: 'Room', sortable: true },
    { key: 'keyType', label: 'Type' },
    { key: 'status', label: 'Status', render: (r) => <StatusBadge status={r.status} /> },
    { key: 'issuedTo', label: 'Issued To', render: (r) => r.issuedTo ?? '-' },
    { key: 'issuedBy', label: 'Issued By', render: (r) => r.issuedBy ?? '-' },
    { key: 'issuedAt', label: 'Issued At', render: (r) => r.issuedAt ?? '-' },
    { key: 'returnedAt', label: 'Returned At', render: (r) => r.returnedAt ?? '-' },
  ];

  const masterColumns: Column<MasterKeyDto>[] = [
    { key: 'name', label: 'Name', sortable: true },
    { key: 'description', label: 'Description' },
    { key: 'accessScope', label: 'Access Scope' },
    { key: 'status', label: 'Status', render: (r) => <StatusBadge status={r.status} /> },
    { key: 'issuedTo', label: 'Issued To', render: (r) => r.issuedTo ?? '-' },
    { key: 'issuedAt', label: 'Issued At', render: (r) => r.issuedAt ?? '-' },
  ];

  return (
    <div className="p-6">
      <div className="mb-6 flex items-center justify-between">
        <h1 className="text-2xl font-semibold text-primary">Room Keys</h1>
        <div className="flex gap-3">
          {tab === 'room' && (
            <button onClick={() => setShowIssue(true)} className="rounded-lg bg-accent px-5 py-2.5 text-sm font-medium text-white hover:opacity-90">+ Issue Key</button>
          )}
          {tab === 'master' && (
            <button onClick={() => setShowCreateMaster(true)} className="rounded-lg bg-accent px-5 py-2.5 text-sm font-medium text-white hover:opacity-90">+ Create Master Key</button>
          )}
        </div>
      </div>

      <div className="mb-4 flex gap-2">
        <button onClick={() => setTab('room')} className={`rounded-lg px-4 py-2 text-sm ${tab === 'room' ? 'bg-accent text-white' : 'border border-primary/10 text-primary/60 hover:bg-primary/5'}`}>Room Keys</button>
        <button onClick={() => setTab('master')} className={`rounded-lg px-4 py-2 text-sm ${tab === 'master' ? 'bg-accent text-white' : 'border border-primary/10 text-primary/60 hover:bg-primary/5'}`}>Master Keys</button>
      </div>

      {error && <div className="mb-4 rounded-lg bg-red-50 p-3 text-sm text-red-600">{error}</div>}
      {success && <div className="mb-4 rounded-lg bg-emerald-50 p-3 text-sm text-emerald-600">{success}</div>}

      {tab === 'room' && (
        <DataTable
          columns={roomColumns}
          data={keys}
          keyExtractor={(r) => r.id}
          searchKeys={['roomNumber', 'issuedTo', 'issuedBy']}
          searchPlaceholder="Search keys..."
          loading={loading}
          actions={(row) => (
            <div className="flex gap-2">
              {row.status === 'Issued' && (
                <>
                  <button onClick={() => handleReturnKey(row.id)} disabled={saving} className="rounded-lg border border-emerald-200 px-3 py-1.5 text-xs text-emerald-600 hover:bg-emerald-50">Return</button>
                  <button onClick={() => handleMarkLost(row.id)} disabled={saving} className="rounded-lg border border-amber-200 px-3 py-1.5 text-xs text-amber-600 hover:bg-amber-50">Lost</button>
                </>
              )}
              <button onClick={() => setDeleteKeyTarget(row)} className="rounded-lg border border-rose-200 px-3 py-1.5 text-xs text-rose-600 hover:bg-rose-50">Delete</button>
            </div>
          )}
        />
      )}

      {tab === 'master' && (
        <DataTable
          columns={masterColumns}
          data={masterKeys}
          keyExtractor={(r) => r.id}
          searchKeys={['name', 'description', 'issuedTo']}
          searchPlaceholder="Search master keys..."
          loading={loading}
          actions={(row) => (
            <div className="flex gap-2">
              {row.status === 'Available' && (
                <button onClick={() => { setIssueMasterId(row.id); setIssueMasterTo(''); setShowIssueMaster(true); }} className="rounded-lg border border-accent/30 px-3 py-1.5 text-xs text-accent hover:bg-accent/5">Issue</button>
              )}
              {row.status === 'Issued' && (
                <button onClick={() => handleReturnMaster(row.id)} disabled={saving} className="rounded-lg border border-emerald-200 px-3 py-1.5 text-xs text-emerald-600 hover:bg-emerald-50">Return</button>
              )}
              <button onClick={() => setDeleteMasterTarget(row)} className="rounded-lg border border-rose-200 px-3 py-1.5 text-xs text-rose-600 hover:bg-rose-50">Delete</button>
            </div>
          )}
        />
      )}

      <Modal open={showIssue} onClose={() => setShowIssue(false)} title="Issue Room Key">
        <div className="space-y-4">
          <div>
            <label className="mb-1 block text-xs font-medium text-primary/60">Room</label>
            <select value={issueRoomId} onChange={(e) => {
              const id = Number(e.target.value);
              setIssueRoomId(id);
              const room = rooms.find((r) => r.id === id);
              setIssueRoomNumber(room?.roomNumber ?? '');
            }} className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent">
              <option value={0}>Select room...</option>
              {rooms.filter((r) => r.status === 'Available' || r.status === 'Occupied').map((r) => (
                <option key={r.id} value={r.id}>Room {r.roomNumber} ({r.type})</option>
              ))}
            </select>
          </div>
          <div>
            <label className="mb-1 block text-xs font-medium text-primary/60">Issued To (Guest Name)</label>
            <input value={issueTo} onChange={(e) => setIssueTo(e.target.value)} className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
          <div>
            <label className="mb-1 block text-xs font-medium text-primary/60">Issued By (Staff Name)</label>
            <input value={issueBy} onChange={(e) => setIssueBy(e.target.value)} className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
        </div>
        <div className="mt-6 flex justify-end gap-3">
          <button onClick={() => setShowIssue(false)} className="rounded-lg border border-primary/10 px-5 py-2.5 text-sm text-primary/60 hover:bg-primary/5">Cancel</button>
          <button onClick={handleIssueKey} disabled={saving || !issueRoomId || !issueTo || !issueBy} className="rounded-lg bg-accent px-5 py-2.5 text-sm text-white hover:opacity-90 disabled:opacity-50">
            {saving ? 'Issuing...' : 'Issue Key'}
          </button>
        </div>
      </Modal>

      <Modal open={showCreateMaster} onClose={() => setShowCreateMaster(false)} title="Create Master Key">
        <div className="space-y-4">
          <div>
            <label className="mb-1 block text-xs font-medium text-primary/60">Name</label>
            <input value={mkName} onChange={(e) => setMkName(e.target.value)} className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
          <div>
            <label className="mb-1 block text-xs font-medium text-primary/60">Description</label>
            <input value={mkDesc} onChange={(e) => setMkDesc(e.target.value)} className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
          </div>
          <div>
            <label className="mb-1 block text-xs font-medium text-primary/60">Access Scope</label>
            <select value={mkScope} onChange={(e) => setMkScope(e.target.value)} className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent">
              <option value="">Select scope...</option>
              <option value="1">Floor 1</option>
              <option value="2">Floor 2</option>
              <option value="3">Floor 3</option>
              <option value="1-3">All Floors (1-3)</option>
              <option value="Suite">Suite Rooms</option>
              <option value="All">All Rooms</option>
            </select>
          </div>
        </div>
        <div className="mt-6 flex justify-end gap-3">
          <button onClick={() => setShowCreateMaster(false)} className="rounded-lg border border-primary/10 px-5 py-2.5 text-sm text-primary/60 hover:bg-primary/5">Cancel</button>
          <button onClick={handleCreateMaster} disabled={saving || !mkName || !mkScope} className="rounded-lg bg-accent px-5 py-2.5 text-sm text-white hover:opacity-90 disabled:opacity-50">
            {saving ? 'Creating...' : 'Create'}
          </button>
        </div>
      </Modal>

      <Modal open={showIssueMaster} onClose={() => setShowIssueMaster(false)} title="Issue Master Key" size="sm">
        <div>
          <label className="mb-1 block text-xs font-medium text-primary/60">Issued To</label>
          <input value={issueMasterTo} onChange={(e) => setIssueMasterTo(e.target.value)} className="w-full rounded-lg border border-primary/10 px-4 py-2.5 text-sm outline-none focus:border-accent" />
        </div>
        <div className="mt-6 flex justify-end gap-3">
          <button onClick={() => setShowIssueMaster(false)} className="rounded-lg border border-primary/10 px-5 py-2.5 text-sm text-primary/60 hover:bg-primary/5">Cancel</button>
          <button onClick={handleIssueMaster} disabled={saving || !issueMasterTo} className="rounded-lg bg-accent px-5 py-2.5 text-sm text-white hover:opacity-90 disabled:opacity-50">
            {saving ? 'Issuing...' : 'Issue'}
          </button>
        </div>
      </Modal>

      <ConfirmDeleteModal
        open={!!deleteKeyTarget}
        onClose={() => setDeleteKeyTarget(null)}
        onConfirm={handleDeleteKey}
        title="Delete Key"
        message={`Delete key for Room ${deleteKeyTarget?.roomNumber}?`}
      />

      <ConfirmDeleteModal
        open={!!deleteMasterTarget}
        onClose={() => setDeleteMasterTarget(null)}
        onConfirm={handleDeleteMaster}
        title="Delete Master Key"
        message={`Delete master key "${deleteMasterTarget?.name}"?`}
      />
    </div>
  );
}
