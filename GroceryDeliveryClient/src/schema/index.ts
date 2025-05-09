import { z } from 'zod';

export const ProductSchema = z.object({
	productId: z.number(),
	productName: z.string(),
	categoryId: z.number(),
	price: z.number(),
	stockQuantity: z.number(),
	imagePath: z.string().nullable(),
	description: z.string().optional(),
});

export const CategorySchema = z.object({
	id: z.number(),
	name: z.string()
});

export const ShoppingBasketItemSchema = z.object({
	product: ProductSchema,
	quantity: z.number(),
});

export const ShoppingBasketSchema = z.array(ShoppingBasketItemSchema);

export const OrderSchema = z.object({
	// id: z.number(),
	userId: z.number(),
	// customerName: z.string(),
	// customerEmail: z.string(),
	// customerPhone: z.string(),
	address: z.string(),
	city: z.string(),
	zipCode: z.number(),
	country: z.string(),
	paymentMethod: z.string(),
	// orderStatus: z.string(),
	// totalAmount: z.number(),
});