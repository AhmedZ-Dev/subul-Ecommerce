// features/payment-method/index.ts
// Public barrel — the ONLY way to import from the payment-method feature.

export { PaymentMethodListingPage } from './components/pages/payment-method-listing-page';
export { PaymentMethodForm } from './components/pages/payment-method-form';
export { PaymentMethodView } from './components/pages/payment-method-view';

export {
  usePaymentMethods,
  usePaymentMethod,
  paymentMethodKeys,
} from './hooks/usePaymentMethod';
export {
  useCreatePaymentMethod,
  useUpdatePaymentMethod,
  useDeletePaymentMethod,
  useChangePaymentMethodStatus,
} from './hooks/usePaymentMethodMutations';

export type {
  PaymentMethodDto,
  PaymentMethodListItem,
  PaymentMethodQueryParams,
  PaymentMethodStatus,
  PaymentMethodType,
} from './types';

export {
  createPaymentMethodSchema,
  updatePaymentMethodSchema,
} from './schemas/payment-method.schema';
export type {
  CreatePaymentMethodInput,
  UpdatePaymentMethodInput,
} from './schemas/payment-method.schema';

export { getPaymentMethods, getPaymentMethodById } from './api/payment-method.api';
export { getCachedPaymentMethodById } from './api/payment-method.cached';

export {
  paymentMethodListingParsers,
  paymentMethodListingSearchParamsCache,
} from './search-params';
