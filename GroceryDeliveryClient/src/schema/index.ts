import { z } from 'zod';

export const ProductSchema = z.object({
	id: z.number(),
	name: z.string(),
	price: z.number(),
	image: z.string().nullable(),
	description: z.string().optional(),
	stock: z.number()
});

export const ShoppingBasketItemSchema = z.object({
	product: ProductSchema,
	quantity: z.number(),
});

export const ShoppingBasketSchema = z.array(ShoppingBasketItemSchema);
