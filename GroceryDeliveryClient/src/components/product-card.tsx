import { Button } from '@/components/ui/button';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import type { Product } from '@/types';
import { useState, type FC } from 'react';
import PlaceholderImage from '@/assets/placeholder-image.svg';
import { Link } from 'react-router-dom';
import { Stepper } from '@/components/stepper';
import { useShoppingBasket } from '@/hooks/shopping-basket';

type ProductCardProps = {
	product: Product;
};

export const ProductCard: FC<ProductCardProps> = ({ product }) => {
	const { id, name, image, price, description, stock } = product;
	const inStock = (stock > 0);
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
		if (quantity === product.stock) {
			return;
		}
		setQuantity(quantity + 1);
		return;
	};

	return (
		<Card className="w-full max-w-sm rounded-2xl shadow-md p-0 overflow-hidden">
			<CardHeader className="block h-56 p-0 w-full relative bg-background">
				<Link to={`/products/${String(id)}`} className='absolute w-full h-full'>
				</Link>
				<img
					src={image || PlaceholderImage}
					alt={name}
					className="absolute w-full h-full object-cover mix-blend-difference"
				/>
			</CardHeader>
			<CardContent className="px-4">
				<Link to={`/products/${String(id)}`}>
					<CardTitle className="text-xl font-semibold text-primary">{name}</CardTitle>
				</Link>
				{description && <p className="text-sm text-muted-foreground mt-1">{description}</p>}
				<div className='grid grid-cols-2 items-center mt-2'>
					<p className="text-lg font-bold text-green-600">${price.toFixed(2)}</p>
					<div className='flex w-full justify-end items-center'>
						{!inStock
							? (<p className="text-sm text-red-500 font-medium">Out of stock</p>)
							: (<p>Stock: {product.stock}</p>)
						}
					</div>
				</div>
			</CardContent>
			<CardFooter className="grid grid-flow-col grid-col-auto gap-2 p-4 pt-0">
				<Stepper
					count={quantity}
					min={0}
					max={product.stock}
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