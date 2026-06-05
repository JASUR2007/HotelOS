import { useEffect, useState } from 'react';
import GenericListPage from './GenericListPage';
import { fetchAuditLogRecords } from '../../api';
import type { AuditLogRecord } from '../../types';

export default function AuditLogs() {
  const [logs, setLogs] = useState<AuditLogRecord[]>([]);

  useEffect(() => {
    fetchAuditLogRecords().then(setLogs).catch(() => undefined);
  }, []);

  return (
    <GenericListPage eyebrow="Governance" title="Audit logs viewer">
      <div className="space-y-4">
        {logs.map((log) => (
          <div key={log.id} className="rounded-2xl border border-primary/10 bg-white p-6 shadow-sm">
            <div className="flex items-center justify-between gap-4">
              <strong>{log.actor}</strong>
              <span className="text-xs uppercase tracking-[0.22em] text-primary/40">{log.createdAt}</span>
            </div>
            <p className="mt-2 text-sm text-primary/65">{log.action}</p>
            <p className="mt-1 text-sm text-primary/50">{log.entity}</p>
          </div>
        ))}
      </div>
    </GenericListPage>
  );
}
