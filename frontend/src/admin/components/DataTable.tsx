import { useEffect, useState, useMemo, type ReactNode } from 'react';
import { useSearchParams } from 'react-router-dom';

export interface Column<T> {
  key: string;
  label: string;
  sortable?: boolean;
  render?: (row: T) => ReactNode;
}

interface DataTableProps<T> {
  columns: Column<T>[];
  data: T[];
  keyExtractor: (row: T) => string | number;
  searchable?: boolean;
  searchPlaceholder?: string;
  searchKeys?: (keyof T)[];
  actions?: (row: T) => ReactNode;
  toolbar?: ReactNode;
  pageSize?: number;
  loading?: boolean;
}

export default function DataTable<T extends Record<string, any>>({
  columns,
  data,
  keyExtractor,
  searchable = true,
  searchPlaceholder = 'Search...',
  searchKeys,
  actions,
  toolbar,
  pageSize = 10,
  loading = false,
}: DataTableProps<T>) {
  const [searchParams] = useSearchParams();
  const querySearch = searchParams.get('q') ?? '';
  const [search, setSearch] = useState(querySearch);
  const [sortKey, setSortKey] = useState<string | null>(null);
  const [sortDir, setSortDir] = useState<'asc' | 'desc'>('asc');
  const [page, setPage] = useState(0);

  useEffect(() => {
    setSearch(querySearch);
    setPage(0);
  }, [querySearch]);

  const filtered = useMemo(() => {
    if (!search || !searchKeys) return data;
    const q = search.toLowerCase();
    return data.filter((row) =>
      searchKeys.some((key) => String(row[key] ?? '').toLowerCase().includes(q)),
    );
  }, [data, search, searchKeys]);

  const sorted = useMemo(() => {
    if (!sortKey) return filtered;
    return [...filtered].sort((a, b) => {
      const av = a[sortKey] ?? '';
      const bv = b[sortKey] ?? '';
      const cmp = typeof av === 'number' && typeof bv === 'number' ? av - bv : String(av).localeCompare(String(bv));
      return sortDir === 'asc' ? cmp : -cmp;
    });
  }, [filtered, sortKey, sortDir]);

  const totalPages = Math.ceil(sorted.length / pageSize);
  const paged = sorted.slice(page * pageSize, (page + 1) * pageSize);

  function handleSort(key: string) {
    if (sortKey === key) {
      setSortDir((d) => (d === 'asc' ? 'desc' : 'asc'));
    } else {
      setSortKey(key);
      setSortDir('asc');
    }
    setPage(0);
  }

  return (
    <div>
      {(searchable || toolbar) && (
        <div className="mb-4 flex flex-wrap items-center justify-between gap-4">
          {toolbar}
          {searchable && (
            <input
              type="text"
              placeholder={searchPlaceholder}
              value={search}
              onChange={(e) => { setSearch(e.target.value); setPage(0); }}
              className="rounded-lg border border-primary/10 bg-white px-4 py-2 text-sm outline-none focus:border-accent"
            />
          )}
        </div>
      )}

      <div className="overflow-hidden rounded-xl border border-primary/10 bg-white shadow-sm">
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-primary/10">
            <thead className="bg-[#f7f3ee]">
              <tr>
                {columns.map((col) => (
                  <th
                    key={col.key}
                    onClick={col.sortable ? () => handleSort(col.key) : undefined}
                    className={`px-5 py-4 text-xs uppercase tracking-[0.2em] text-primary/50 ${
                      col.sortable ? 'cursor-pointer select-none hover:text-primary' : ''
                    }`}
                  >
                    <span className="flex items-center gap-1">
                      {col.label}
                      {sortKey === col.key && (
                        <span className="text-accent">{sortDir === 'asc' ? '↑' : '↓'}</span>
                      )}
                    </span>
                  </th>
                ))}
                {actions && <th className="px-5 py-4 text-xs uppercase tracking-[0.2em] text-primary/50">Actions</th>}
              </tr>
            </thead>
            <tbody className="divide-y divide-primary/10">
              {loading ? (
                <tr>
                  <td colSpan={columns.length + (actions ? 1 : 0)} className="px-5 py-12 text-center text-sm text-primary/40">
                    <div className="flex justify-center">
                      <div className="h-6 w-6 animate-spin rounded-full border-2 border-accent border-t-transparent" />
                    </div>
                  </td>
                </tr>
              ) : paged.length === 0 ? (
                <tr>
                  <td colSpan={columns.length + (actions ? 1 : 0)} className="px-5 py-12 text-center text-sm text-primary/40">
                    No data found
                  </td>
                </tr>
              ) : (
                paged.map((row) => (
                  <tr key={keyExtractor(row)} className="transition hover:bg-primary/5">
                    {columns.map((col) => (
                      <td key={col.key} className="px-5 py-4 text-sm text-primary">
                        {col.render ? col.render(row) : row[col.key]}
                      </td>
                    ))}
                    {actions && (
                      <td className="px-5 py-4">
                        <div className="flex items-center gap-2">{actions(row)}</div>
                      </td>
                    )}
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {totalPages > 1 && (
        <div className="mt-4 flex items-center justify-between">
          <p className="text-xs text-primary/40">
            Showing {page * pageSize + 1}–{Math.min((page + 1) * pageSize, sorted.length)} of {sorted.length}
          </p>
          <div className="flex gap-1">
            <button
              onClick={() => setPage((p) => Math.max(0, p - 1))}
              disabled={page === 0}
              className="rounded-lg border border-primary/10 px-3 py-1.5 text-sm text-primary/60 hover:bg-primary/5 disabled:opacity-30"
            >
              ‹
            </button>
            {Array.from({ length: totalPages }, (_, i) => (
              <button
                key={i}
                onClick={() => setPage(i)}
                className={`rounded-lg border px-3 py-1.5 text-sm ${
                  i === page
                    ? 'border-accent bg-accent text-white'
                    : 'border-primary/10 text-primary/60 hover:bg-primary/5'
                }`}
              >
                {i + 1}
              </button>
            ))}
            <button
              onClick={() => setPage((p) => Math.min(totalPages - 1, p + 1))}
              disabled={page === totalPages - 1}
              className="rounded-lg border border-primary/10 px-3 py-1.5 text-sm text-primary/60 hover:bg-primary/5 disabled:opacity-30"
            >
              ›
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
