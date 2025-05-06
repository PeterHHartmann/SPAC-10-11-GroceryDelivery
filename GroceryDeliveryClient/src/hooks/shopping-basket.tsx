import { ShoppingBasketSchema } from '@/schema';
import type { ShoppingBasket } from '@/types';
import { createContext, use, useCallback, useEffect, useMemo, useState } from 'react';

type ShoppingBasketProviderProps = { children: React.ReactNode; };
type ShoppingCartActionData = {
	product: string,
	quantity: number;
};

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

const ShoppingBasketContext = createContext<
	{
		basket: ShoppingBasket,
		addToBasket: ({ product, quantity }: ShoppingCartActionData) => void;
		removeFromBasket: ({ product, quantity }: ShoppingCartActionData) => void;
	} | undefined
>(undefined);

export const ShoppingBasketProvider = ({ children }: ShoppingBasketProviderProps) => {
	// const [state, dispatch] = useReducer(basketReducer, { basket: encodeBasket() || [] });
	const [basket, setBasket] = useState<ShoppingBasket>(encodeBasket() || []);

	const addToBasket = useCallback(({ product, quantity }: ShoppingCartActionData) => {
		const basketCopy = basket.slice();
		// Find index of product if its already in cart
		// console.log('we got here', index);
		const index = basketCopy.findIndex(item => item.product === product);
		if (index !== -1) {
			console.log('found item in cart');
			// If found, increase the quantity
			basketCopy[index].quantity += quantity;
		} else {
			console.log('item not in cart');
			// If not found, add new product
			basketCopy.push({ product, quantity });
		}
		setBasket(basketCopy);
		return;
	}, [basket]);

	const removeFromBasket = useCallback(({ product, quantity }: ShoppingCartActionData): void => {
		const basketCopy = basket.slice();
		// Find index of product in cart
		const index = basketCopy.findIndex(item => item.product === product);

		if (index !== -1) {
			const updatedAmount = basketCopy[index].quantity - quantity;
			if (updatedAmount > 0) {
				// Update quantity
				basketCopy[index].quantity = updatedAmount;
			} else {
				// Remove item if quantity is 0 or less
				basketCopy.splice(index, 1);
			}
		}
		return;
	}, [basket]);

	// Memoized value to be supplied to the context provider
	const contextValue = useMemo(() => {
		return { basket, addToBasket, removeFromBasket };
	}, [basket, addToBasket, removeFromBasket]);

	// useEffect to update localstorage when basket state changes
	useEffect(() => {
		decodeBasket(basket);
	}, [basket]);

	return (
		// eslint-disable-next-line react-x/no-context-provider
		<ShoppingBasketContext.Provider value={contextValue}>
			{children}
		</ShoppingBasketContext.Provider>
	);
};

export const useShoppingBasket = () => {
	const context = use(ShoppingBasketContext);
	if (context === undefined) {
		throw new Error('useShoppingBasket must be used within a ShoppingBasketProvider');
	}
	return context;
};