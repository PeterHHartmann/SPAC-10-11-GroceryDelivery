import { ShoppingBasketSchema } from '@/schema';
import type { ShoppingBasket } from '@/types';
import { useEffect, useReducer } from 'react';

const decodeBasket = (basket: ShoppingBasket): void => {
	try {
		const parsed = ShoppingBasketSchema.parse(basket);
		localStorage.setItem('basket', JSON.stringify(parsed));
	} catch (err) {
		console.error('Invalid basket data', err);
	}
};

const encodeBasket = (): ShoppingBasket | null => {
	const item = localStorage.getItem('basket');
	if (item === null) {
		return null;
	}
	try {
		return ShoppingBasketSchema.parse(JSON.parse(item));
	} catch {
		return null;
	}
};

type BasketAction = 'add' | 'remove';

const basketReducer = (
	state: ShoppingBasket,
	action: {
		type: BasketAction,
		data: {
			product: string,
			amount: number;
		};
	}): ShoppingBasket => {
	switch (action.type) {
		case 'add': {
			return addAction(state, action.data);
		}
		case 'remove': {
			return removeAction(state, action.data);
		}
		default:
			return state;
	}
};

const addAction = (cart: ShoppingBasket, data: { product: string, amount: number; }): ShoppingBasket => {
	const { product, amount } = data;

	// Find index of product if its already in cart
	const index = cart.findIndex(item => item.product === product);

	// 
	if (index !== -1) {
		// If found, increase the amount
		cart[index].amount += amount;
	} else {
		// If not found, add new product
		cart.push({ product, amount });
	}

	return cart;
};

const removeAction = (
	cart: ShoppingBasket,
	data: {
		product: string,
		amount: number;
	}): ShoppingBasket => {
	const { product, amount } = data;
	// Find index of product in cart
	const index = cart.findIndex(item => item.product === product);

	if (index !== -1) {
		const updatedAmount = cart[index].amount - amount;
		if (updatedAmount > 0) {
			// Update amount
			cart[index].amount = updatedAmount;
		} else {
			// Remove item if amount is 0 or less
			cart.splice(index, 1);
		}
	}

	return cart;
};

export const useShoppingBasket = () => {
	const [basket, basketDispatch] = useReducer(basketReducer, encodeBasket() || []);

	useEffect(() => {
		decodeBasket(basket);
	}, [basket]);

	return [basket, basketDispatch];
};