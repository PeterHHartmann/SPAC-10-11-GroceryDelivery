import { z } from 'zod';

export const ShoppingBasketSchema = z.array(z.object({
	product: z.string(),
	amount: z.number(),
}));
