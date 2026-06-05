interface BarChartProps {
  data: { label: string; value: number; color?: string }[];
  height?: number;
}

export function BarChart({ data, height = 200 }: BarChartProps) {
  const max = Math.max(...data.map((d) => d.value), 1);
  return (
    <div className="flex items-end gap-3" style={{ height }}>
      {data.map((d) => (
        <div key={d.label} className="flex flex-1 flex-col items-center gap-1.5">
          <span className="text-xs font-medium text-primary">{d.value}</span>
          <div
            className="w-full rounded-t-md transition-all"
            style={{
              height: `${(d.value / max) * 100}%`,
              backgroundColor: d.color ?? '#a37d4c',
              minHeight: 4,
            }}
          />
          <span className="text-[10px] text-primary/50">{d.label}</span>
        </div>
      ))}
    </div>
  );
}

interface DonutChartProps {
  data: { label: string; value: number; color: string }[];
  size?: number;
}

export function DonutChart({ data, size = 160 }: DonutChartProps) {
  const total = data.reduce((s, d) => s + d.value, 0) || 1;
  const segments = data.reduce<(number | string)[]>((acc, d) => {
    const pct = (d.value / total) * 100;
    if (acc.length === 0) {
      acc.push(pct);
    } else {
      acc.push((acc[acc.length - 1] as number) + pct);
    }
    return acc;
  }, []);

  const conicGradient = data
    .map((d, i) => {
      const start = i === 0 ? 0 : segments[i - 1];
      const end = segments[i];
      return `${d.color} ${start}% ${end}%`;
    })
    .join(', ');

  return (
    <div className="flex items-center gap-6">
      <div
        className="rounded-full"
        style={{
          width: size,
          height: size,
          background: `conic-gradient(${conicGradient})`,
        }}
      />
      <div className="space-y-2">
        {data.map((d) => (
          <div key={d.label} className="flex items-center gap-2">
            <span className="h-3 w-3 rounded-sm" style={{ backgroundColor: d.color }} />
            <span className="text-xs text-primary/70">{d.label}</span>
            <span className="text-xs font-medium text-primary">{d.value}</span>
          </div>
        ))}
      </div>
    </div>
  );
}

interface StatCardProps {
  label: string;
  value: string;
  change?: string;
  trend?: 'up' | 'down' | 'stable';
  icon?: string;
}

export function StatCard({ label, value, change, trend, icon }: StatCardProps) {
  const trendColors = {
    up: 'text-emerald-600 bg-emerald-50',
    down: 'text-rose-600 bg-rose-50',
    stable: 'text-amber-600 bg-amber-50',
  };
  return (
    <div className="rounded-xl border border-primary/10 bg-white p-5 shadow-sm">
      <div className="flex items-start justify-between">
        <div>
          <p className="text-xs uppercase tracking-[0.2em] text-primary/40">{label}</p>
          <p className="mt-2 text-3xl font-bold text-primary">{value}</p>
        </div>
        {icon && <span className="text-2xl">{icon}</span>}
      </div>
      {change && trend && (
        <p className={`mt-3 inline-flex items-center gap-1 rounded-full px-2.5 py-0.5 text-xs font-medium ${trendColors[trend]}`}>
          {trend === 'up' ? '↑' : trend === 'down' ? '↓' : '→'} {change}
        </p>
      )}
    </div>
  );
}
