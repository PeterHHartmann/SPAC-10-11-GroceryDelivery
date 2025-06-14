import type { CategorySchema, ProductSchema, ShoppingBasketItemSchema, ShoppingBasketSchema, OrderSchema, UserSchema } from '@/schema';
import { z } from 'zod';

export type PaginationParams = {
	page: number,
	pageSize: number;
};

export type ApiError = {
	message: string;
	code: string;
	status: number;
};

export type Product = z.infer<typeof ProductSchema>;

export type ProductQueryParams = {
	searchTerm?: string;
	categoryId?: Category['id'];
};

export type ShoppingBasketItem = z.infer<typeof ShoppingBasketItemSchema>;

export type ShoppingBasket = z.infer<typeof ShoppingBasketSchema>;

export type Category = z.infer<typeof CategorySchema>;

export type Order = z.infer<typeof OrderSchema> & {
	orderItems: { productId: number, quantity: number; }[];
};

export type User = z.infer<typeof UserSchema>;