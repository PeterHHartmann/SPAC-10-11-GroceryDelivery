import { ShoppingBasketSchema } from '@/schema';
import type { Product, ShoppingBasket } from '@/types';
import { createContext, use, useCallback, useEffect, useMemo, useState } from 'react';

type ShoppingBasketProviderProps = { children: React.ReactNode; };
type ShoppingBasketItem = {
	product: Product,
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
		addToBasket: (item: ShoppingBasketItem) => void;
		removeFromBasket: (item: ShoppingBasketItem) => void;
		ClearBasket: (item: ShoppingBasketItem) => void;
	} | undefined
>(undefined);

export const ShoppingBasketProvider = ({ children }: ShoppingBasketProviderProps) => {
	// const [state, dispatch] = useReducer(basketReducer, { basket: encodeBasket() || [] });
	const [basket, setBasket] = useState<ShoppingBasket>(encodeBasket() || []);

	const addToBasket = useCallback(({ product, quantity }: ShoppingBasketItem) => {
		const basketCopy = basket.slice();
		// Find index of product if its already in basket
		// console.log('we got here', index);
		const index = basketCopy.findIndex(item => item.product.productId === product.productId);
		if (index !== -1) {
			console.log('found item in basket');
			// If found, increase the quantity
			let updatedQuantity = basketCopy[index].quantity + quantity;
			if (updatedQuantity > product.stockQuantity) {
				updatedQuantity = product.stockQuantity;
			}
			basketCopy[index].quantity = updatedQuantity;
		} else {
			console.log('item not in basket');
			// If not found, add new product
			basketCopy.push({ product, quantity });
		}
		setBasket(basketCopy);
		return;
	}, [basket]);

	const removeFromBasket = useCallback(({ product, quantity }: ShoppingBasketItem): void => {
		console.log('remove called');

		const basketCopy = basket.slice();
		// Find index of product in basket
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
		setBasket(basketCopy);
		return;
	}, [basket]);

	const clearBasket = useCallback((): void => {
		setBasket([]);
	}, []);

	

	// Memoized value to be supplied to the context provider
	const contextValue = useMemo(() => {
		return { basket, addToBasket, removeFromBasket, clearBasket };
	}, [basket, addToBasket, removeFromBasket, clearBasket]);

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