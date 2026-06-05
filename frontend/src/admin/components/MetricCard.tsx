import type { AdminMetric } from '../../types';

interface MetricCardProps { metric: AdminMetric }

export default function MetricCard({ metric }: MetricCardProps) {
  const toneClass = metric.trend === 'up' ? 'text-emerald-700 bg-emerald-100' : metric.trend === 'down' ? 'text-rose-700 bg-rose-100' : 'text-amber-700 bg-amber-100';
  return (
    <article className="border border-primary/10 bg-white p-6 shadow-sm">
      <p className="text-xs uppercase tracking-[0.28em] text-primary/45">{metric.label}</p>
      <div className="mt-4 flex items-end justify-between gap-3">
        <strong className="text-4xl font-primary text-primary">{metric.value}</strong>
        <span className={`rounded-full px-3 py-1 text-xs font-semibold uppercase tracking-[0.2em] ${toneClass}`}>{metric.delta}</span>
      </div>
    </article>
  );
}
