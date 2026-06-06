import { useEffect, useState } from 'react';
import GenericListPage from './GenericListPage';
import { fetchEventLogRecords } from '../../api';
import type { EventLogRecord } from '../../types';

export default function EventLogs() {
  const [logs, setLogs] = useState<EventLogRecord[]>([]);

  useEffect(() => {
    fetchEventLogRecords().then(setLogs).catch(() => undefined);
  }, []);

  return (
    <GenericListPage eyebrow="Messaging" title="Event logs viewer">
      <div className="space-y-4">
        {logs.map((log) => (
          <div key={log.id} className="rounded-2xl border border-primary/10 bg-white p-6 shadow-sm">
            <div className="flex items-center justify-between gap-4">
              <strong>{log.eventName}</strong>
              <span className="text-xs uppercase tracking-[0.22em] text-black">{log.status}</span>
            </div>
            <p className="mt-2 text-sm text-primary/65">Routing key: {log.routingKey}</p>
            <p className="mt-1 text-sm text-primary/50">{log.createdAt}</p>
          </div>
        ))}
      </div>
    </GenericListPage>
  );
}
