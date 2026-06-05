import { useEffect, useState } from 'react';
import { fetchReportsSummary, fetchRevenueAnalytics, fetchOccupancyAnalytics } from '../../api';
import { BarChart } from '../components/Charts';
import type { ReportSummary, RevenuePoint, AnalyticsPoint } from '../../types';

type DateRange = '7d' | '30d' | '90d';

export default function Reports() {
  const [summary, setSummary] = useState<ReportSummary[]>([]);
  const [revenue, setRevenue] = useState<RevenuePoint[]>([]);
  const [occupancy, setOccupancy] = useState<AnalyticsPoint[]>([]);
  const [range, setRange] = useState<DateRange>('30d');
  const [loading, setLoading] = useState(true);

  async function load() {
    setLoading(true);
    try {
      const [s, r, o] = await Promise.all([
        fetchReportsSummary(),
        fetchRevenueAnalytics(),
        fetchOccupancyAnalytics(),
      ]);
      setSummary(s);
      setRevenue(r);
      setOccupancy(o);
    } catch { /* fallback */ }
    setLoading(false);
  }

  useEffect(() => { load(); }, []);

  function handleExportCSV() {
    const rows = [
      ['Label', 'Value', 'Subtitle'],
      ...summary.map((s) => [s.label, s.value, s.subtitle]),
      [],
      ['Revenue (30d)'],
      ...revenue.map((r) => [r.label, String(r.value)]),
    ];
    const csv = rows.map((r) => r.join(',')).join('\n');
    const blob = new Blob([csv], { type: 'text/csv' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url; a.download = `hotelos-report-${range}.csv`; a.click();
    URL.revokeObjectURL(url);
  }

  const filteredRevenue = revenue.slice(range === '7d' ? -7 : range === '30d' ? -30 : undefined);

  return (
    <div className="px-6 py-8">
      <div className="mb-8 flex items-center justify-between">
        <div>
          <p className="text-xs uppercase tracking-[0.35em] text-accent">Analytics</p>
          <h1 className="mt-2 text-2xl font-bold text-primary">Reports</h1>
        </div>
        <div className="flex items-center gap-3">
          <div className="flex rounded-lg border border-primary/10 overflow-hidden">
            {(['7d', '30d', '90d'] as DateRange[]).map((r) => (
              <button key={r} onClick={() => setRange(r)}
                className={`px-4 py-2 text-xs font-medium ${range === r ? 'bg-accent text-white' : 'text-primary/60 hover:bg-primary/5'}`}>
                {r}
              </button>
            ))}
          </div>
          <button onClick={handleExportCSV}
            className="rounded-lg border border-accent/30 px-4 py-2 text-xs font-medium text-accent hover:bg-accent/5">
            Export CSV
          </button>
        </div>
      </div>

      {loading ? (
        <p className="py-12 text-center text-sm text-primary/40">Loading reports...</p>
      ) : (
        <>
          <div className="grid gap-4 md:grid-cols-3 mb-8">
            {summary.map((item) => (
              <div key={item.label} className="rounded-2xl border border-primary/10 bg-white p-6 shadow-sm">
                <p className="text-xs uppercase tracking-[0.28em] text-primary/40">{item.label}</p>
                <div className="mt-2 text-3xl font-bold text-primary">{item.value}</div>
                <p className="mt-2 text-sm text-primary/65">{item.subtitle}</p>
              </div>
            ))}
          </div>

          <div className="grid gap-6 lg:grid-cols-2">
            <div className="rounded-2xl border border-primary/10 bg-white p-6">
              <h3 className="mb-4 text-sm font-semibold uppercase tracking-wider text-primary/60">Revenue Trend</h3>
              <BarChart data={filteredRevenue.map((d) => ({ ...d, color: '#a37d4c' }))} height={200} />
            </div>

            <div className="rounded-2xl border border-primary/10 bg-white p-6">
              <h3 className="mb-4 text-sm font-semibold uppercase tracking-wider text-primary/60">Occupancy Breakdown</h3>
              <BarChart data={occupancy.map((d, i) => ({ ...d, color: ['#a37d4c', '#6b8c5e', '#b56b6b'][i % 3] }))} height={200} />
            </div>
          </div>
        </>
      )}
    </div>
  );
}
