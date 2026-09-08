export default function AuthLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="auth-scene relative flex min-h-dvh items-center justify-center overflow-hidden p-6">
      <div aria-hidden className="pointer-events-none absolute inset-0">
        <span className="auth-blob auth-blob-1" />
        <span className="auth-blob auth-blob-2" />
        <span className="auth-blob auth-blob-3" />
        <span className="auth-blob auth-blob-4" />
      </div>

      <div
        aria-hidden
        className="auth-stage pointer-events-none absolute inset-x-6 inset-y-10 hidden rounded-3xl md:block lg:inset-x-24 lg:inset-y-16"
      />

      <div className="relative z-10 w-full max-w-sm">{children}</div>
    </div>
  );
}
