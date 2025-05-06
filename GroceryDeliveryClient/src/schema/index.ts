import { z } from 'zod';

export const ShoppingBasketSchema = z.array(z.object({
	product: z.string(),
	quantity: z.number(),
}));
