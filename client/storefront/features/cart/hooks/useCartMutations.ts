"use client"

import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import {
  addCartItem,
  clearCart,
  removeCartItem,
  updateCartItem,
} from "../api/cart.api"
import { cartKeys } from "./useCart"
import type { AddToCartPayload, Cart, UpdateCartItemPayload } from "../types"
import { messages } from "@/lib/messages.ar"

export function useAddToCart() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: AddToCartPayload) => addCartItem(payload),
    onSuccess: (cart) => {
      queryClient.setQueryData(cartKeys.active(), cart)
      queryClient.invalidateQueries({ queryKey: cartKeys.all })
    },
    onError: (error: Error) => {
      toast.error(error.message ?? messages.cart.addError)
    },
  })
}

export function useUpdateCartItem() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({
      itemId,
      payload,
    }: {
      itemId: number
      payload: UpdateCartItemPayload
    }) => updateCartItem(itemId, payload),
    onMutate: async ({ itemId, payload }) => {
      await queryClient.cancelQueries({ queryKey: cartKeys.active() })
      const previous = queryClient.getQueryData<Cart>(cartKeys.active())

      if (previous) {
        queryClient.setQueryData<Cart>(cartKeys.active(), {
          ...previous,
          items: previous.items.map((item) =>
            item.id === itemId
              ? {
                  ...item,
                  quantity: payload.quantity,
                  lineTotal: item.unitPrice * payload.quantity,
                }
              : item,
          ),
        })
      }

      return { previous }
    },
    onError: (error: Error, _vars, context) => {
      if (context?.previous) {
        queryClient.setQueryData(cartKeys.active(), context.previous)
      }
      toast.error(error.message ?? messages.cart.updateError)
    },
    onSuccess: (cart) => {
      queryClient.setQueryData(cartKeys.active(), cart)
    },
  })
}

export function useRemoveCartItem() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (itemId: number) => removeCartItem(itemId),
    onMutate: async (itemId) => {
      await queryClient.cancelQueries({ queryKey: cartKeys.active() })
      const previous = queryClient.getQueryData<Cart>(cartKeys.active())

      if (previous) {
        queryClient.setQueryData<Cart>(cartKeys.active(), {
          ...previous,
          items: previous.items.filter((item) => item.id !== itemId),
          itemCount: Math.max(0, previous.itemCount - 1),
        })
      }

      return { previous }
    },
    onError: (error: Error, _vars, context) => {
      if (context?.previous) {
        queryClient.setQueryData(cartKeys.active(), context.previous)
      }
      toast.error(error.message ?? messages.cart.removeError)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: cartKeys.all })
      toast.success(messages.cart.removeSuccess)
    },
  })
}

export function useClearCart() {
  const queryClient = useQueryClient()

  return () => {
    clearCart()
    queryClient.removeQueries({ queryKey: cartKeys.all })
  }
}
