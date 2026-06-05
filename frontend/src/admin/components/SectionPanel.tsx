import type { ReactNode } from 'react';

interface SectionPanelProps { title: string; subtitle?: string; children: ReactNode }

export default function SectionPanel({ title, subtitle, children }: SectionPanelProps) {
  return (
    <section className="border border-primary/10 bg-white p-6 shadow-sm">
      <div className="mb-5">
        <h3 className="h3">{title}</h3>
        {subtitle ? <p className="mt-1 text-sm text-primary/60">{subtitle}</p> : null}
      </div>
      {children}
    </section>
  );
}
