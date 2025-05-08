import { useProduct } from '@/api/queries/product-queries';
import { useMemo, useState, type FC } from 'react';
import { useParams } from 'react-router-dom';
import { useShoppingBasket } from '@/hooks/shopping-basket';
import { Stepper } from '@/components/stepper';
import { Button } from '@/components/ui/button';
import type { Product } from '@/types';
import { useCategory } from '@/api/queries/category-queries';
import { ApiImage } from '@/components/api-image';

export const ProductDetailPage: FC = () => {

	const { productId } = useParams<{ productId: string; }>();

	const {
		data: product,
		isLoading,
		error
	} = useProduct(productId ? parseInt(productId, 10) : 0);

	if (isLoading) {
		return null;
	}

	if (error || !product) {
		return null;
	}

	return (
		<ProductDetailCard product={product} />
	);
};

type ProductDetailCardProps = {
	product: Product;
};

const ProductDetailCard: FC<ProductDetailCardProps> = ({ product }) => {
	const { addToBasket } = useShoppingBasket();
	const [quantity, setQuantity] = useState<number>(1);

	const inStock = useMemo(() => {
		return product.stockQuantity;
	}, [product]);

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
		if (quantity === product.stockQuantity) {
			return;
		}
		setQuantity(quantity + 1);
		return;
	};

	return (
		<div className="max-w-3xl h-max mx-auto p-6 bg-white shadow-md rounded-lg mt-4">
			<div className="flex flex-col md:flex-row gap-6">

				<div className="flex-shrink-0">
					<ApiImage src={product.imagePath} className='w-64 h-64 object-cover rounded-md' />
				</div>

				<div className="flex-1">
					<h1 className="text-2xl font-bold mb-2">{product.productName}</h1>
					<ProductCategoryText categoryId={product.categoryId} />
					<p className="text-lg font-semibold text-green-600 mb-2">${product.price.toFixed(2)}</p>
					<p className={`mb-4 ${product.stockQuantity > 0 ? "text-blue-600" : "text-red-500"}`}>
						{product.stockQuantity > 0 ? `${String(product.stockQuantity)} in stock` : "Out of stock"}
					</p>
					{product.description && (
						<div className="mt-4">
							<h2 className="text-lg font-medium mb-1">Description</h2>
							<p className="text-gray-700">{product.description}</p>
						</div>
					)}

					<div className='grid grid-flow-col grid-col-auto gap-2 pt-4'>
						<Stepper
							count={quantity}
							min={0}
							max={product.stockQuantity}
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
					</div>
				</div>
			</div>
		</div>
	);
};

type ProductCategoryTextProps = {
	categoryId: number;
};

const ProductCategoryText: FC<ProductCategoryTextProps> = ({ categoryId }) => {

	const { data: category, isLoading, error } = useCategory(categoryId);

	const style = 'text-gray-600 mb-4';

	if (isLoading) {
		return (
			<p className={style}></p>
		);
	}

	if (error || !category) {
		return (
			<p className={style}></p>
		);
	}

	return (
		<p className={style}>{category.name}</p>
	);

};