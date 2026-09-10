'use client';

import { useState } from 'react';
import Image from 'next/image';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { signIn } from 'next-auth/react';
import { useRouter } from 'next/navigation';
import { AlertCircle, Eye, EyeOff, Loader2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { messages } from '@/lib/messages.ar';
import { loginSchema, type LoginInput } from '../../schemas/login.schema';

function getLoginErrorMessage(code: string | undefined) {
  const match = /^rate-limited-(\d+)$/.exec(code ?? '');

  if (match) {
    return messages.auth.loginRateLimited(Number(match[1]));
  }

  return messages.auth.loginError;
}

/* حقل أبيض فوق الزجاج الفاتح: حدّ ظاهر للفصل ونص داكن بتباين عالٍ */
const fieldClassName =
  'h-11 touch-manipulation rounded-xl border-stone-500 bg-white/90 px-3.5 text-stone-900 shadow-sm focus-visible:border-stone-700 focus-visible:ring-3 focus-visible:ring-stone-700/70 disabled:bg-white/60 aria-invalid:border-red-500 aria-invalid:ring-red-500/25 dark:bg-white/90';

export function LoginForm() {
  const router = useRouter();
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  const form = useForm<LoginInput>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      email: '',
      password: '',
    },
  });

  async function onSubmit(values: LoginInput) {
    setError(null);
    setIsSubmitting(true);
    try {
      const result = await signIn('credentials', {
        email: values.email,
        password: values.password,
        redirect: false,
      });
      if (result?.error) {
        setError(getLoginErrorMessage(result.code));
      } else {
        router.replace('/dashboard');
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="auth-glass w-full rounded-3xl p-7 sm:p-8">
      <div className="mb-7 text-center">
        <Image
          src="/assets/logo_subul-brand_full_20260829_black.png"
          alt={messages.common.companyName}
          width={128}
          height={128}
          priority
          className="mx-auto h-16 w-16 object-contain"
        />
        <h1 className="mt-4 text-2xl font-semibold tracking-tight text-stone-900">
          {messages.auth.loginTitle}
        </h1>
        <p className="mt-1.5 text-sm text-stone-600">{messages.auth.loginDescription}</p>
      </div>

      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-5" noValidate>
          <FormField
            control={form.control}
            name="email"
            render={({ field }) => (
              <FormItem className="gap-2">
                <FormLabel className="text-sm font-medium text-stone-800">
                  {messages.auth.emailLabel}
                </FormLabel>
                <FormControl>
                  <Input
                    {...field}
                    type="email"
                    autoComplete="email"
                    autoFocus
                    dir="ltr"
                    disabled={isSubmitting}
                    className={fieldClassName}
                  />
                </FormControl>
                <FormMessage className="text-red-700" />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="password"
            render={({ field }) => (
              <FormItem className="gap-2">
                <FormLabel className="text-sm font-medium text-stone-800">
                  {messages.auth.passwordLabel}
                </FormLabel>
                <div className="relative" dir="ltr">
                  <FormControl>
                    <Input
                      {...field}
                      type={showPassword ? 'text' : 'password'}
                      autoComplete="current-password"
                      dir="ltr"
                      disabled={isSubmitting}
                      className={`${fieldClassName} pe-12`}
                    />
                  </FormControl>
                  <button
                    type="button"
                    onClick={() => setShowPassword((value) => !value)}
                    disabled={isSubmitting}
                    aria-pressed={showPassword}
                    aria-label={
                      showPassword ? messages.auth.hidePassword : messages.auth.showPassword
                    }
                    className="absolute inset-y-0 end-1.5 my-auto flex size-9 touch-manipulation items-center justify-center rounded-lg text-stone-600 transition-colors hover:bg-stone-900/10 hover:text-stone-900 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-stone-700 disabled:pointer-events-none disabled:opacity-50"
                  >
                    {showPassword ? (
                      <EyeOff className="size-[1.125rem]" aria-hidden />
                    ) : (
                      <Eye className="size-[1.125rem]" aria-hidden />
                    )}
                  </button>
                </div>
                <FormMessage className="text-red-700" />
              </FormItem>
            )}
          />

          <div aria-live="polite">
            {error ? (
              <p
                className="flex items-start gap-2 rounded-xl border border-red-300 bg-red-50/90 px-3 py-2.5 text-sm text-red-800"
                role="alert"
              >
                <AlertCircle className="mt-px size-4 shrink-0" aria-hidden />
                <span>{error}</span>
              </p>
            ) : null}
          </div>

          <Button
            type="submit"
            disabled={isSubmitting}
            aria-busy={isSubmitting}
            className="h-11 w-full touch-manipulation rounded-xl bg-stone-900 text-base font-medium text-white shadow-lg transition-colors hover:bg-stone-800 focus-visible:ring-3 focus-visible:ring-stone-900/40 focus-visible:ring-offset-2 disabled:opacity-70 md:text-sm"
          >
            {isSubmitting ? (
              <>
                <Loader2 className="size-4 animate-spin" aria-hidden />
                {messages.auth.loggingIn}
              </>
            ) : (
              messages.auth.loginButton
            )}
          </Button>
        </form>
      </Form>
    </div>
  );
}
