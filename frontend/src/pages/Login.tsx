import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';

export default function Login() {
	const navigate = useNavigate();
	const user = useAuthStore((state) => state.user);
	const login = useAuthStore((state) => state.login);
	const [email, setEmail] = useState('admin@hotelos.local');
	const [password, setPassword] = useState('admin123');
	const [status, setStatus] = useState('');
	const [error, setError] = useState('');
	const [submitting, setSubmitting] = useState(false);

	useEffect(() => {
		if (user && user.role !== 'Guest') {
			navigate('/admin', { replace: true });
		}
	}, [navigate, user]);

	async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
		event.preventDefault();
		setError('');
		setStatus('');
		setSubmitting(true);
		try {
			const user = await login(email, password);
			setStatus(`Signed in as ${user.displayName} (${user.role})`);
			navigate(user.role === 'Guest' ? '/' : '/admin');
		} catch (err) {
			setError(err instanceof Error ? err.message : 'Login failed');
		} finally {
			setSubmitting(false);
		}
	}

	return (
		<section className="container mx-auto max-w-xl px-4 py-28">
			<div className="mb-8 text-center">
				<p className="text-sm uppercase tracking-[0.35em] text-accent">HotelOS</p>
				<h1 className="h2 mt-3">Login</h1>
			</div>

			<form onSubmit={handleSubmit} className="border border-primary/10 bg-white p-8 shadow-sm">
				<label className="block text-sm uppercase tracking-[0.25em] text-primary/50">
					Email
					<input className="mt-2 w-full border border-primary/10 px-4 py-3 outline-none focus:border-accent" value={email} onChange={(e) => setEmail(e.target.value)} type="email" />
				</label>
				<label className="mt-5 block text-sm uppercase tracking-[0.25em] text-primary/50">
					Password
					<input className="mt-2 w-full border border-primary/10 px-4 py-3 outline-none focus:border-accent" value={password} onChange={(e) => setPassword(e.target.value)} type="password" />
				</label>
				<button type="submit" disabled={submitting} className="btn btn-primary btn-lg mt-6 w-full">
					{submitting ? 'Signing in...' : 'Sign in'}
				</button>
				{error ? <p className="mt-4 text-center text-sm text-red-600">{error}</p> : null}
				{status ? <p className="mt-4 text-center text-sm text-emerald-700">{status}</p> : null}
			</form>
			<p className="mt-6 text-center text-sm text-primary/50">
				No account?{' '}
				<Link to="/register" className="text-accent hover:underline">Register</Link>
			</p>
		</section>
	);
}