import Image from 'next/image';
import { messages } from '@/lib/messages.ar';

export default function AuthLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="flex min-h-svh flex-col items-center justify-center bg-muted/40 p-6">
      <div className="mb-8 text-center">
        <h1 className="sr-only">{messages.common.companyName}</h1>
        <Image
          src="/assets/logo_subul-brand_full_20260829_black.png"
          alt={messages.common.companyName}
          width={112}
          height={112}
          priority
          className="mx-auto h-28 w-28 object-contain dark:hidden"
        />
        <Image
          src="/assets/logo_subul-brand_full_20260829_white.png"
          alt={messages.common.companyName}
          width={112}
          height={112}
          priority
          className="mx-auto hidden h-28 w-28 object-contain dark:block"
        />
        <p className="mt-3 text-sm text-muted-foreground">{messages.auth.loginDescription}</p>
      </div>
      {children}
    </div>
  );
}
