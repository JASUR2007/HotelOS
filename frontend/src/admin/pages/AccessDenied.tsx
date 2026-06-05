import { Link } from 'react-router-dom';

export default function AccessDenied() {
  return (
    <section className="flex min-h-[60vh] items-center justify-center px-6">
      <div className="text-center">
        <p className="text-sm uppercase tracking-[0.35em] text-accent">403</p>
        <h1 className="h2 mt-2">Access Denied</h1>
        <p className="mx-auto mt-4 max-w-md text-primary/60">
          You do not have the required permissions to access this page.
          Contact your administrator to request access.
        </p>
        <Link
          to="/admin/dashboard"
          className="btn btn-primary mx-auto mt-8 inline-flex w-auto px-10"
        >
          Back to Dashboard
        </Link>
      </div>
    </section>
  );
}
