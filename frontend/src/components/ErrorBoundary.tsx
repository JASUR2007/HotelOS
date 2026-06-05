import { Component, type ReactNode } from 'react';

interface Props {
	children: ReactNode;
	fallback?: ReactNode;
}

interface State {
	hasError: boolean;
	error: Error | null;
}

export default class ErrorBoundary extends Component<Props, State> {
	constructor(props: Props) {
		super(props);
		this.state = { hasError: false, error: null };
	}

	static getDerivedStateFromError(error: Error): State {
		return { hasError: true, error };
	}

	render() {
		if (this.state.hasError) {
			return (
				this.props.fallback ?? (
					<section className="container mx-auto max-w-xl px-4 py-28 text-center">
						<p className="text-sm uppercase tracking-[0.35em] text-red-500">Error</p>
						<h1 className="h2 mt-3">Something went wrong</h1>
						<p className="mt-4 text-sm text-primary/50">
							{this.state.error?.message ?? 'An unexpected error occurred.'}
						</p>
						<button
							onClick={() => window.location.reload()}
							className="btn btn-primary mt-6 inline-block"
						>
							Reload Page
						</button>
					</section>
				)
			);
		}

		return this.props.children;
	}
}
