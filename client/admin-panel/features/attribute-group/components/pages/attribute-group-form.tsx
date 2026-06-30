'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useForm, useFieldArray, type Resolver } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { Loader2, Plus, Save, Trash2, X } from 'lucide-react';
import { toast } from 'sonner';

import { Button } from '@/components/ui/button';
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Checkbox } from '@/components/ui/checkbox';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { FormActionsBar } from '@/components/layout/form-actions-bar';

import {
  createAttributeGroupSchema,
  type CreateAttributeGroupFormValues,
} from '../../schemas/attribute-group.schema';
import { useCreateAttributeGroup, useUpdateAttributeGroup } from '../../hooks/useAttributeGroupMutations';
import type { AttributeGroupDto } from '../../types';
import { messages } from '@/lib/messages.ar';

const m = messages.attributeGroup.form;
const f = messages.attributeGroup.fields;

function getDefaultValues(initialData?: AttributeGroupDto | null): CreateAttributeGroupFormValues {
  if (initialData) {
    return {
      nameEn: initialData.nameEn,
      nameAr: initialData.nameAr ?? '',
      slug: initialData.slug ?? '',
      sortOrder: initialData.sortOrder,
      isFilterable: initialData.isFilterable,
      attributes: initialData.attributes.map((attr) => ({
        id: attr.id,
        nameEn: attr.nameEn,
        nameAr: attr.nameAr ?? '',
        slug: attr.slug ?? '',
        unit: attr.unit ?? '',
        inputType: attr.inputType as 'text' | 'select' | 'boolean' | 'number',
        isFilterable: attr.isFilterable,
        sortOrder: attr.sortOrder,
      })),
    };
  }

  return {
    nameEn: '',
    nameAr: '',
    slug: '',
    sortOrder: 0,
    isFilterable: true,
    attributes: [],
  };
}

interface AttributeGroupFormProps {
  initialData?: AttributeGroupDto | null;
}

export function AttributeGroupForm({ initialData }: AttributeGroupFormProps) {
  const router = useRouter();
  const { mutateAsync: createGroupAsync, isPending: isCreating } = useCreateAttributeGroup();
  const { mutateAsync: updateGroupAsync, isPending: isUpdating } = useUpdateAttributeGroup();
  const isEditMode = !!initialData;
  const isPending = isCreating || isUpdating;
  const [leaveOpen, setLeaveOpen] = useState(false);

  const form = useForm<CreateAttributeGroupFormValues>({
    resolver: zodResolver(createAttributeGroupSchema) as Resolver<CreateAttributeGroupFormValues>,
    defaultValues: getDefaultValues(initialData),
  });

  const { isDirty } = form.formState;

  const { fields, append, remove } = useFieldArray({
    name: 'attributes',
    control: form.control,
  });

  useEffect(() => {
    if (initialData) {
      form.reset(getDefaultValues(initialData));
    }
  }, [initialData, form.reset]);

  useEffect(() => {
    if (!isDirty) return;
    const onBeforeUnload = (e: BeforeUnloadEvent) => {
      e.preventDefault();
    };
    window.addEventListener('beforeunload', onBeforeUnload);
    return () => window.removeEventListener('beforeunload', onBeforeUnload);
  }, [isDirty]);

  function buildPayload(values: CreateAttributeGroupFormValues) {
    return {
      ...values,
      nameAr: values.nameAr || undefined,
      slug: values.slug || undefined,
      attributes: values.attributes.map((attr) => ({
        ...attr,
        nameAr: attr.nameAr || undefined,
        slug: attr.slug || undefined,
        unit: attr.unit || undefined,
        id: attr.id ?? undefined,
      })),
    };
  }

  async function onSubmit(values: CreateAttributeGroupFormValues) {
    const payload = buildPayload(values);

    try {
      if (isEditMode && initialData) {
        await updateGroupAsync({ id: initialData.id, payload });
      } else {
        await createGroupAsync(payload);
      }

      toast.success(
        isEditMode
          ? messages.attributeGroup.updateSuccess
          : messages.attributeGroup.createSuccess,
      );
      router.push('/attribute-groups');
    } catch {
      // Errors are surfaced via mutation onError toasts.
    }
  }

  function handleCancel() {
    if (isDirty) {
      setLeaveOpen(true);
      return;
    }
    router.push('/attribute-groups');
  }

  return (
    <>
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
          <div className="grid gap-6 lg:grid-cols-[1fr_300px]">
            <div className="space-y-6">
              <Card className="shadow-xs">
                <CardHeader>
                  <CardTitle className="text-base">{m.general_info}</CardTitle>
                </CardHeader>
                <CardContent className="grid gap-4">
                  <FormField
                    control={form.control}
                    name="nameEn"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{f.nameEn} *</FormLabel>
                        <FormControl>
                          <Input
                            id="attribute-group-name-en"
                            disabled={isPending}
                            placeholder="Size"
                            dir="ltr"
                            {...field}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="nameAr"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{f.nameAr}</FormLabel>
                        <FormControl>
                          <Input
                            id="attribute-group-name-ar"
                            disabled={isPending}
                            placeholder="الحجم"
                            dir="rtl"
                            {...field}
                            value={field.value ?? ''}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="slug"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{f.slug}</FormLabel>
                        <FormControl>
                          <Input
                            id="attribute-group-slug"
                            disabled={isPending}
                            placeholder="size"
                            dir="ltr"
                            {...field}
                            value={field.value ?? ''}
                          />
                        </FormControl>
                        <FormDescription>{m.slug_hint}</FormDescription>
                        {field.value && (
                          <p className="text-muted-foreground font-mono text-xs" dir="ltr">
                            {m.slugPreview(field.value)}
                          </p>
                        )}
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </CardContent>
              </Card>

              <Card className="shadow-xs">
                <CardHeader className="flex flex-row items-center justify-between">
                  <CardTitle className="text-base">{m.attributes}</CardTitle>
                  <Button
                    type="button"
                    variant="outline"
                    size="sm"
                    onClick={() =>
                      append({
                        nameEn: '',
                        nameAr: '',
                        slug: '',
                        unit: '',
                        inputType: 'text',
                        isFilterable: true,
                        sortOrder: fields.length,
                      })
                    }
                    disabled={isPending}
                  >
                    <Plus className="size-4" />
                    {m.add_attribute}
                  </Button>
                </CardHeader>
                <CardContent className="space-y-4">
                  {fields.length === 0 && (
                    <div className="text-muted-foreground flex min-h-24 items-center justify-center rounded-lg border border-dashed border-border/70 bg-muted/10 px-4 py-6 text-center text-sm">
                      {m.no_attributes}
                    </div>
                  )}
                  {fields.map((field, index) => (
                    <div
                      key={field.id}
                      className="relative flex flex-col gap-4 rounded-lg border border-border/50 bg-muted/20 p-4 md:grid md:grid-cols-12 md:items-start"
                    >
                      <div className="col-span-11 grid grid-cols-1 gap-4 md:grid-cols-3">
                        <FormField
                          control={form.control}
                          name={`attributes.${index}.nameEn`}
                          render={({ field }) => (
                            <FormItem>
                              <FormLabel>{f.nameEn} *</FormLabel>
                              <FormControl>
                                <Input disabled={isPending} dir="ltr" {...field} />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                        <FormField
                          control={form.control}
                          name={`attributes.${index}.nameAr`}
                          render={({ field }) => (
                            <FormItem>
                              <FormLabel>{f.nameAr}</FormLabel>
                              <FormControl>
                                <Input
                                  disabled={isPending}
                                  dir="rtl"
                                  {...field}
                                  value={field.value ?? ''}
                                />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                        <FormField
                          control={form.control}
                          name={`attributes.${index}.inputType`}
                          render={({ field }) => (
                            <FormItem>
                              <FormLabel>{f.inputType}</FormLabel>
                              <Select
                                disabled={isPending}
                                onValueChange={field.onChange}
                                value={field.value}
                                dir="ltr"
                              >
                                <FormControl>
                                  <SelectTrigger>
                                    <SelectValue />
                                  </SelectTrigger>
                                </FormControl>
                                <SelectContent>
                                  <SelectItem value="text">Text</SelectItem>
                                  <SelectItem value="select">Select</SelectItem>
                                  <SelectItem value="boolean">Boolean</SelectItem>
                                  <SelectItem value="number">Number</SelectItem>
                                </SelectContent>
                              </Select>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                        <FormField
                          control={form.control}
                          name={`attributes.${index}.slug`}
                          render={({ field }) => (
                            <FormItem>
                              <FormLabel>{f.slug}</FormLabel>
                              <FormControl>
                                <Input
                                  disabled={isPending}
                                  dir="ltr"
                                  {...field}
                                  value={field.value ?? ''}
                                />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                        <FormField
                          control={form.control}
                          name={`attributes.${index}.unit`}
                          render={({ field }) => (
                            <FormItem>
                              <FormLabel>{f.unit}</FormLabel>
                              <FormControl>
                                <Input
                                  disabled={isPending}
                                  dir="ltr"
                                  {...field}
                                  value={field.value ?? ''}
                                />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                        <div className="flex items-end gap-4">
                          <FormField
                            control={form.control}
                            name={`attributes.${index}.sortOrder`}
                            render={({ field }) => (
                              <FormItem className="flex-1">
                                <FormLabel>{f.sortOrder}</FormLabel>
                                <FormControl>
                                  <Input
                                    disabled={isPending}
                                    type="number"
                                    dir="ltr"
                                    {...field}
                                  />
                                </FormControl>
                                <FormMessage />
                              </FormItem>
                            )}
                          />
                          <FormField
                            control={form.control}
                            name={`attributes.${index}.isFilterable`}
                            render={({ field }) => (
                              <FormItem className="flex flex-col items-center justify-center rounded-lg border p-3">
                                <FormLabel className="mb-2 text-xs">{f.isFilterable}</FormLabel>
                                <FormControl>
                                  <Checkbox
                                    checked={field.value}
                                    onCheckedChange={field.onChange}
                                    disabled={isPending}
                                  />
                                </FormControl>
                              </FormItem>
                            )}
                          />
                        </div>
                      </div>
                      <div className="col-span-1 flex justify-end pt-8">
                        <Button
                          type="button"
                          variant="ghost"
                          size="icon-sm"
                          onClick={() => remove(index)}
                          className="text-destructive hover:bg-destructive/10 hover:text-destructive"
                          disabled={isPending}
                          aria-label={messages.common.delete}
                        >
                          <Trash2 className="size-4" />
                        </Button>
                      </div>
                    </div>
                  ))}
                </CardContent>
              </Card>
            </div>

            <div className="space-y-6">
              <Card className="shadow-xs">
                <CardHeader>
                  <CardTitle className="text-base">{m.settings}</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <FormField
                    control={form.control}
                    name="sortOrder"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>{f.sortOrder}</FormLabel>
                        <FormControl>
                          <Input
                            id="attribute-group-sort-order"
                            disabled={isPending}
                            type="number"
                            dir="ltr"
                            {...field}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="isFilterable"
                    render={({ field }) => (
                      <FormItem className="flex flex-row items-center justify-between rounded-lg border p-4">
                        <div className="space-y-0.5">
                          <FormLabel className="text-base">{f.isFilterable}</FormLabel>
                        </div>
                        <FormControl>
                          <Checkbox
                            checked={field.value}
                            onCheckedChange={field.onChange}
                            disabled={isPending}
                          />
                        </FormControl>
                      </FormItem>
                    )}
                  />
                </CardContent>
              </Card>
            </div>
          </div>

          <FormActionsBar>
            <Button
              id="attribute-group-submit-btn"
              type="submit"
              disabled={isPending}
              className="min-w-[130px]"
            >
              {isPending ? (
                <>
                  <Loader2 className="size-4 animate-spin" />
                  {m.saving}
                </>
              ) : (
                <>
                  <Save className="size-4" />
                  {isEditMode ? m.update : m.create}
                </>
              )}
            </Button>
            <Button
              id="attribute-group-cancel-btn"
              type="button"
              variant="outline"
              disabled={isPending}
              onClick={handleCancel}
            >
              <X className="size-4" />
              {m.cancel}
            </Button>
          </FormActionsBar>
        </form>
      </Form>

      <AlertDialog open={leaveOpen} onOpenChange={setLeaveOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{m.unsavedTitle}</AlertDialogTitle>
            <AlertDialogDescription>{m.unsavedDescription}</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>{m.unsavedStay}</AlertDialogCancel>
            <AlertDialogAction onClick={() => router.push('/attribute-groups')}>
              {m.unsavedLeave}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
