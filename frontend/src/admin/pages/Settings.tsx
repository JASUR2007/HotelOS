import { useEffect, useState } from 'react';
import { NavLink } from 'react-router-dom';
import { fetchSettings, updateSetting } from '../../api';
import type { AppSetting } from '../../types';

const SETTING_OPTIONS: Record<string, string[]> = {
  Notifications: ['Enabled', 'Disabled'],
  Payments: ['Live', 'Test', 'Disabled'],
  RBAC: ['Strict', 'Standard', 'Disabled'],
  Infrastructure: ['Healthy', 'Degraded', 'Maintenance', 'Offline'],
  'Payment Provider': ['Live', 'Test', 'Disabled'],
  'Access Control': ['Strict', 'Standard', 'Disabled'],
};

export default function Settings() {
  const [settings, setSettings] = useState<AppSetting[]>([]);
  const [editing, setEditing] = useState<string | null>(null);
  const [editValue, setEditValue] = useState('');
  const [saved, setSaved] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchSettings().then(setSettings).catch(() => undefined);
  }, []);

  function startEdit(key: string, current: string) {
    setEditing(key);
    setEditValue(current);
    setSaved(false);
  }

  async function persistSetting(key: string, value: string) {
    const savedSetting = await updateSetting(key, value);
    setSettings((prev) => prev.map((s) => (s.key === key ? savedSetting : s)));
    setSaved(true);
    setTimeout(() => setSaved(false), 2000);
  }

  async function handleSave(key: string) {
    setError('');
    try {
      await persistSetting(key, editValue);
      setEditing(null);
    } catch {
      setError('Failed to save setting. Check backend connection.');
    }
  }

  async function handleToggle(key: string, current: string) {
    const next = current === 'Enabled' ? 'Disabled' : 'Enabled';
    setError('');
    try {
      await persistSetting(key, next);
    } catch {
      setError('Failed to update notifications setting.');
    }
  }

  return (
    <div className="px-6 py-8">
      <div className="mb-8">
        <p className="text-xs uppercase tracking-[0.35em] text-accent">System</p>
        <h1 className="mt-2 text-2xl font-bold text-primary">Settings</h1>
      </div>

      {saved && (
        <div className="mb-4 rounded-xl border border-emerald-200 bg-emerald-50 p-3 text-sm text-emerald-700">
          Settings saved successfully.
        </div>
      )}

      {error && (
        <div className="mb-4 rounded-xl border border-rose-200 bg-rose-50 p-3 text-sm text-rose-700">
          {error}
        </div>
      )}

      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
        {settings.map((setting) => (
          <div key={setting.key} className="rounded-2xl border border-primary/10 bg-white p-6 shadow-sm">
            <p className="text-xs uppercase tracking-[0.28em] text-primary/40">{setting.key}</p>

            {editing === setting.key ? (
              <div className="mt-3">
                <select
                  value={editValue}
                  onChange={(e) => setEditValue(e.target.value)}
                  className="w-full rounded-lg border border-accent px-3 py-2 text-sm font-semibold outline-none"
                  autoFocus
                >
                  {(SETTING_OPTIONS[setting.key] ?? [setting.value]).map((value) => (
                    <option key={value} value={value}>{value}</option>
                  ))}
                </select>
                <div className="mt-3 flex gap-2">
                  <button onClick={() => handleSave(setting.key)}
                    className="rounded-lg bg-accent px-3 py-1.5 text-xs text-white hover:bg-accent/90">Save</button>
                  <button onClick={() => setEditing(null)}
                    className="rounded-lg border border-primary/10 px-3 py-1.5 text-xs text-primary/60 hover:bg-primary/5">Cancel</button>
                </div>
              </div>
            ) : setting.key === 'Notifications' ? (
              <div className="mt-3 flex items-center gap-3">
                <button
                  onClick={() => handleToggle(setting.key, setting.value)}
                  className={`relative inline-flex h-6 w-11 items-center rounded-full transition-colors ${
                    setting.value === 'Enabled' ? 'bg-emerald-500' : 'bg-gray-200'
                  }`}
                >
                  <span className={`inline-block h-4 w-4 transform rounded-full bg-white transition-transform ${
                    setting.value === 'Enabled' ? 'translate-x-6' : 'translate-x-1'
                  }`} />
                </button>
                <span className="text-sm font-medium text-primary">{setting.value}</span>
                <button onClick={() => startEdit(setting.key, setting.value)}
                  className="ml-auto text-xs text-accent hover:underline">Edit</button>
              </div>
            ) : (
              <div className="mt-3">
                <div className="text-xl font-semibold text-primary">{setting.value}</div>
                <div className="mt-2 flex items-center justify-between">
                  <p className="text-sm text-primary/65">{setting.description}</p>
                  <button onClick={() => startEdit(setting.key, setting.value)}
                    className="text-xs text-accent hover:underline">Edit</button>
                </div>
              </div>
            )}
          </div>
        ))}
      </div>

      <div className="mt-10">
        <h2 className="mb-4 text-lg font-bold text-primary">Infrastructure</h2>
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
          {[
            { key: 'RabbitMQ', desc: 'Queues, consumers, messages', link: 'infrastructure/rabbitmq' },
            { key: 'Database', desc: 'Postgres status, connections', link: 'infrastructure/database' },
            { key: 'WebSockets', desc: 'SignalR connections', link: 'infrastructure/websockets' },
            { key: 'Event Logs', desc: 'Routing and delivery', link: 'infrastructure/event-logs' },
          ].map((infra) => {
            const infraSetting = settings.find((s) => s.key === infra.key);
            const currentValue = infraSetting?.value ?? 'Healthy';
            return (
              <div key={infra.key} className="rounded-2xl border border-primary/10 bg-white p-5 shadow-sm">
                <p className="text-xs uppercase tracking-[0.28em] text-primary/40">{infra.key}</p>
                <div className="mt-3">
                  <NavLink to={infra.link} className="text-xs text-accent hover:underline">{infra.key} Dashboard</NavLink>
                  <div className="mt-2">
                    <select
                      value={currentValue}
                      onChange={(e) => {
                        const newVal = e.target.value;
                        setSettings((prev) => {
                          const exists = prev.find((s) => s.key === infra.key);
                          if (exists) return prev.map((s) => s.key === infra.key ? { ...s, value: newVal } : s);
                          return [...prev, { key: infra.key, value: newVal, description: infra.desc }];
                        });
                      }}
                      className="w-full rounded-lg border border-primary/10 px-3 py-2 text-sm font-semibold outline-none focus:border-accent"
                    >
                      {['Healthy', 'Degraded', 'Maintenance', 'Offline'].map((v) => (
                        <option key={v} value={v}>{v}</option>
                      ))}
                    </select>
                  </div>
                  <p className="mt-1 text-xs text-primary/50">{infra.desc}</p>
                </div>
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
}
