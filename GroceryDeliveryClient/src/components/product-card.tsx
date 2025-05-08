import { Button } from '@/components/ui/button';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import type { Product } from '@/types';
import { useState, type FC } from 'react';
import { Link } from 'react-router-dom';
import { Stepper } from '@/components/stepper';
import { useShoppingBasket } from '@/hooks/shopping-basket';
import { ApiImage } from '@/components/api-image';

type ProductCardProps = {
	product: Product;
};

export const ProductCard: FC<ProductCardProps> = ({ product }) => {
	const { productId, productName, imagePath, price, description, stockQuantity } = product;
	const inStock = (stockQuantity > 0);

	const { addToBasket } = useShoppingBasket();

	const [quantity, setQuantity] = useState<number>(1);

	const handleAddToBasket = (): void => {
		addToBasket({ product: product, quantity: quantity });
		return;
	};

	const handleQuantityDecrease = (): void => {
		if (quantity <= 1) {
			return;
		}
		setQuantity(quantity - 1);
		return;
	};

	const handleQuantityIncrease = (): void => {
		if (quantity === stockQuantity) {
			return;
		}
		setQuantity(quantity + 1);
		return;
	};

	return (
		<Card className="w-full max-w-sm rounded-2xl shadow-md p-0 overflow-hidden">
			<CardHeader className="block h-64 p-0 w-full relative bg-card">
				<ApiImage
					src={imagePath}
					alt={productName}
					className='absolute w-full h-full object-cover mix-blend-multiply'
				/>
			</CardHeader>
			<CardContent className="px-4">
				<Link to={`/product/${String(productId)}`}>
					<CardTitle className="text-xl font-semibold text-primary">{productName}</CardTitle>
				</Link>
				{description && <p className="text-sm text-muted-foreground mt-1">{description}</p>}
				<div className='grid grid-cols-2 items-center mt-2'>
					<p className="text-lg font-bold text-green-600">${price.toFixed(2)}</p>
					<div className='flex w-full justify-end items-center'>
						{!inStock
							? (<p className="text-sm text-red-500 font-medium">Out of stock</p>)
							: (<p>Stock: {stockQuantity}</p>)
						}
					</div>
				</div>
			</CardContent>
			<CardFooter className="grid grid-flow-col grid-col-auto gap-2 p-4 pt-0 mt-auto">
				<Stepper
					count={quantity}
					min={0}
					max={stockQuantity}
					minusPressedHandler={handleQuantityDecrease}
					plusPressedHandler={handleQuantityIncrease}
				/>
				<Button
					disabled={!inStock}
					size='lg'
					variant={'default'}
					onClick={() => { handleAddToBasket(); }}
				>
					{inStock ? "Add to Basket" : "Unavailable"}
				</Button>
			</CardFooter>
		</Card>
	);
};