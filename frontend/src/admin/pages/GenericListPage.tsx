import type { ReactNode } from 'react';

interface GenericListPageProps { title: string; eyebrow: string; children: ReactNode }

export default function GenericListPage({ title, eyebrow, children }: GenericListPageProps) {
  return (
    <section className="px-6 py-8">
      <p className="text-sm uppercase tracking-[0.35em] text-accent">{eyebrow}</p>
      <h1 className="h2 mt-2">{title}</h1>
      <div className="mt-6">{children}</div>
    </section>
  );
}
