import { Button } from '@/components/ui/button';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import type { Product } from '@/types';
import { useState, type FC } from 'react';
import PlaceholderImage from '@/assets/placeholder-image.svg';
import { Link } from 'react-router-dom';
import { Stepper } from '@/components/stepper';

type ProductCardProps = {
	product: Product;
	onAddToCart?: (productId: Product['id']) => void;
};

export const ProductCard: FC<ProductCardProps> = ({ product, onAddToCart }) => {
	const { id, name, image, price, description } = product;
	const inStock = (product.stock && product.stock > 0);

	const [count, setCount] = useState<number>(1);

	const handleCountDecrease = (): void => {
		if (count <= 1) {
			return;
		}
		setCount(count - 1);
		return;
	};

	const handleCountIncrease = (): void => {
		if (count === product.stock) {
			return;
		}
		setCount(count + 1);
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
					count={count}
					min={0}
					max={product.stock}
					minusPressedHandler={handleCountDecrease}
					plusPressedHandler={handleCountIncrease}
				/>
				<Button
					className=""
					disabled={!inStock}
					onClick={() => onAddToCart?.(id)}
				>
					{inStock ? "Add to Cart" : "Unavailable"}
				</Button>
			</CardFooter>
		</Card>
	);
};