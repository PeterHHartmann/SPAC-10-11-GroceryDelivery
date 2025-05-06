import type { ShoppingBasketSchema } from '@/schema';
import { z } from 'zod';

export type Product = {
	id: string | number;
	name: string;
	image: string | null;
	price: number;
	description?: string;
	stock?: number;
};

export type ShoppingBasket = z.infer<typeof ShoppingBasketSchema>;