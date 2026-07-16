import { messages } from '@/lib/messages.ar';

export default function AuthLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="flex min-h-svh flex-col items-center justify-center bg-muted/40 p-6">
      <div className="mb-8 text-center">
        <h1 className="text-2xl font-semibold">{messages.common.companyName}</h1>
        <p className="text-sm text-muted-foreground">{messages.auth.loginDescription}</p>
      </div>
      {children}
    </div>
  );
}
