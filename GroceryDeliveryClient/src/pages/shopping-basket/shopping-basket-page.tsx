import { Separator } from '@/components/ui/separator';
import type { Product } from '@/types';
import type { FC } from 'react';
import { Link } from 'react-router-dom';
import PlaceholderImage from '@/assets/placeholder-image.svg';

type BasketItem = {
	product: Product,
	quantity: number;
};

const placeholderProducts: BasketItem[] = [
	{
		product: {
			id: 1,
			name: "Golden Delicious Apple",
			image: null,
			price: 9.99,
		},
		quantity: 4
	},
	{
		product: {
			id: 2,
			name: "Granny Smith Apple",
			image: null,
			price: 8.73,
		},
		quantity: 2
	},
	{
		product: {
			id: 3,
			name: "Yoghurt",
			image: null,
			price: 4.5,
		},
		quantity: 1
	},
	{
		product: {
			id: 4,
			name: "Milk",
			image: null,
			price: 1.25,
		},
		quantity: 9
	},
	{
		product: {
			id: 5,
			name: "Pear",
			image: null,
			price: 9.99,
		},
		quantity: 3
	},
	{
		product: {
			id: 6,
			name: "Organic Banana",
			image: null,
			price: 3,
		},
		quantity: 20
	},
];

const quantityTotal = (price: number, quantity: number): string => {
	const total = price * quantity;
	return total.toFixed(2);
};

export const ShoppingBasketPage: FC = () => {

	return (
		<div className='w-full flex'>
			<section className='grid grid-flow-row w-3/4'>
				{placeholderProducts.map((item) => (
					<div key={`cart-item-${String(item.product.id)}`} className=''>
						<div className='grid grid-cols-4 gap-2 p-4 justify-between items-center'>
							<div className='flex gap-2 items-center'>
								<div className="block aspect-auto h-32 w-32 p-0 relative bg-background">
									<Link to={`/products/${String(item.product.id)}`} className='absolute w-full h-full'>
									</Link>
									<img
										src={item.product.image || PlaceholderImage}
										alt={item.product.name}
										className="absolute w-full h-full object-cover mix-blend-difference"
									/>
								</div>
								<p className='text-lg font-bold'>{item.product.name}</p>
							</div>
							<p className="text-lg font-bold">Quantity: {item.quantity}</p>
							<p className="text-lg font-bold text-green-600">${item.product.price.toFixed(2)}</p>
							<p className="text-lg font-bold text-green-600">${quantityTotal(item.product.price, item.quantity)}</p>

						</div>
						<Separator orientation='horizontal' className='mt-2' />
					</div>
				))}
			</section>
			<Separator orientation='vertical' className='' />
			<aside className='w-1/4'>
				<h2 className='text-xl'>Summary</h2>
				<Separator orientation='horizontal' className='my-2' />
			</aside>
		</div>
	);
};