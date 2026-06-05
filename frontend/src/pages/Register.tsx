import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';

export default function Register() {
	const navigate = useNavigate();
	const [fullName, setFullName] = useState('');
	const [email, setEmail] = useState('');
	const [password, setPassword] = useState('');
	const [confirmPassword, setConfirmPassword] = useState('');
	const [status, setStatus] = useState('');
	const [error, setError] = useState('');

	async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
		event.preventDefault();
		setError('');
		setStatus('');

		if (password !== confirmPassword) {
			setError('Passwords do not match');
			return;
		}

		if (password.length < 6) {
			setError('Password must be at least 6 characters');
			return;
		}

		try {
			const apiBaseUrl = import.meta.env.VITE_HOTEL_API_URL ?? '/api';
			const response = await fetch(`${apiBaseUrl}/auth/register`, {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify({ email, displayName: fullName, password }),
			});

			if (!response.ok) {
				const body = await response.json().catch(() => null);
				throw new Error(body?.message ?? 'Registration failed');
			}

			setStatus('Account created! Redirecting to login...');
			setTimeout(() => navigate('/login'), 1500);
		} catch (err) {
			setError(err instanceof Error ? err.message : 'Registration failed');
		}
	}

	return (
		<section className="container mx-auto max-w-xl px-4 py-28">
			<div className="mb-8 text-center">
				<p className="text-sm uppercase tracking-[0.35em] text-accent">HotelOS</p>
				<h1 className="h2 mt-3">Create Account</h1>
				<p className="mt-2 text-sm text-primary/50">Register as a guest to book rooms and order food.</p>
			</div>

			<form onSubmit={handleSubmit} className="border border-primary/10 bg-white p-8 shadow-sm">
				<label className="block text-sm uppercase tracking-[0.25em] text-primary/50">
					Full Name
					<input
						className="mt-2 w-full border border-primary/10 px-4 py-3 outline-none focus:border-accent"
						value={fullName}
						onChange={(e) => setFullName(e.target.value)}
						type="text"
						required
					/>
				</label>
				<label className="mt-5 block text-sm uppercase tracking-[0.25em] text-primary/50">
					Email
					<input
						className="mt-2 w-full border border-primary/10 px-4 py-3 outline-none focus:border-accent"
						value={email}
						onChange={(e) => setEmail(e.target.value)}
						type="email"
						required
					/>
				</label>
				<label className="mt-5 block text-sm uppercase tracking-[0.25em] text-primary/50">
					Password
					<input
						className="mt-2 w-full border border-primary/10 px-4 py-3 outline-none focus:border-accent"
						value={password}
						onChange={(e) => setPassword(e.target.value)}
						type="password"
						required
					/>
				</label>
				<label className="mt-5 block text-sm uppercase tracking-[0.25em] text-primary/50">
					Confirm Password
					<input
						className="mt-2 w-full border border-primary/10 px-4 py-3 outline-none focus:border-accent"
						value={confirmPassword}
						onChange={(e) => setConfirmPassword(e.target.value)}
						type="password"
						required
					/>
				</label>
				<button type="submit" className="btn btn-primary btn-lg mt-6 w-full">
					Create Account
				</button>
				{error && <p className="mt-4 text-center text-sm text-red-600">{error}</p>}
				{status && <p className="mt-4 text-center text-sm text-emerald-700">{status}</p>}
				<p className="mt-6 text-center text-sm text-primary/50">
					Already have an account?{' '}
					<Link to="/login" className="text-accent hover:underline">Sign in</Link>
				</p>
			</form>
		</section>
	);
}
