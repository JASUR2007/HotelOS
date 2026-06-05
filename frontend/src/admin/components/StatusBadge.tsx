interface StatusBadgeProps { status: string }

export default function StatusBadge({ status }: StatusBadgeProps) {
  const lower = status.toLowerCase();
  const className = lower.includes('critical') || lower.includes('urgent')
    ? 'bg-rose-100 text-rose-700'
    : lower.includes('clean') || lower.includes('online') || lower.includes('complete') || lower.includes('success')
      ? 'bg-emerald-100 text-emerald-700'
      : lower.includes('info')
        ? 'bg-sky-100 text-sky-700'
        : 'bg-amber-100 text-amber-700';

  return <span className={`rounded-full px-3 py-1 text-xs font-semibold uppercase tracking-[0.2em] ${className}`}>{status}</span>;
}
