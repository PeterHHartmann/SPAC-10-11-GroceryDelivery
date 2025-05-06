import type { ProductSchema, ShoppingBasketItemSchema, ShoppingBasketSchema } from '@/schema';
import { z } from 'zod';

export type Product = z.infer<typeof ProductSchema>;
export type ShoppingBasketItem = z.infer<typeof ShoppingBasketItemSchema>;
export type ShoppingBasket = z.infer<typeof ShoppingBasketSchema>;