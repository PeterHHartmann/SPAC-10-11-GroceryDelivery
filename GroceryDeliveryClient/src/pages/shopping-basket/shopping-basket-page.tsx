import { Separator } from '@/components/ui/separator';
import type { Product } from '@/types';
import { useState, type FC } from 'react';
import PlaceholderImage from '@/assets/placeholder-image.svg';
import { Button } from '@/components/ui/button';
import { Link } from 'react-router-dom';

type BasketItem = {
	product: Product,
	quantity: number;
};

const shoppingBasket: BasketItem[] = [
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

const PACKING_FEE = 4.51;
const DELIVERY_FEE = 10.99;
const SALES_TAX_PERCENT = 25;

const quantityTotal = (price: number, quantity: number): string => {
	const total = price * quantity;
	return total.toFixed(2);
};

const calcSubtotal = (): number => {
	return shoppingBasket.reduce((total, item) => {
		return total + item.product.price * item.quantity;
	}, 0);
};

const calcTotal = (price: number): number => {
	const withSalesTax = price * (1 + SALES_TAX_PERCENT / 100);
	return withSalesTax + PACKING_FEE + DELIVERY_FEE;
};

export const ShoppingBasketPage: FC = () => {

	const [subtotal] = useState<number>(calcSubtotal());

	return (
		<div className='w-full flex'>
			<section className='grid grid-flow-row w-3/4'>
				{shoppingBasket.map((item) => (
					<div key={`cart-item-${String(item.product.id)}`} className=''>
						<div className='grid grid-cols-5 gap-2 p-4 justify-between items-center'>
							<div className="block aspect-auto h-32 w-32 p-0 relative bg-background">
								<img
									src={item.product.image || PlaceholderImage}
									alt={item.product.name}
									className="absolute w-full h-full object-cover mix-blend-difference"
								/>
							</div>
							<p className='text-lg font-bold'>{item.product.name}</p>
							<p className="text-lg font-bold">Quantity: {item.quantity}</p>
							<p className="text-lg font-bold text-green-600">${item.product.price.toFixed(2)}</p>
							<p className="text-lg font-bold text-green-600">${quantityTotal(item.product.price, item.quantity)}</p>

						</div>
						<Separator orientation='horizontal' className='mt-2' />
					</div>
				))}
			</section>
			<Separator orientation='vertical' className='mt-auto' />
			<aside className='w-1/4'>
				<div className='sticky top-14 flex flex-col justify-between bg-accent h-[calc(100vh-3.5rem)]'>
					<div>
						<h2 className='text-xl p-4'>Summary</h2>
						<Separator orientation='horizontal' />
						<div className='grid grid-rows-3 w-full gap-2 py-2'>

							<div className='w-full grid grid-cols-2 px-4'>
								<p className='font-bold'>Subtotal</p>
								<p className='font-bold ml-auto'>${subtotal.toFixed(2)}</p>
							</div>
							<div className='w-full grid grid-cols-2 px-4'>
								<p>Packing fee</p>
								<p className='ml-auto'>${PACKING_FEE.toFixed(2)}</p>
							</div>
							<div className='w-full grid grid-cols-2 px-4'>
								<p>Delivery fee</p>
								<p className='ml-auto'>${DELIVERY_FEE.toFixed(2)}</p>
							</div>
							<div className='w-full grid grid-cols-2 px-4'>
								<p>Sales tax</p>
								<p className='ml-auto'>{SALES_TAX_PERCENT}%</p>
							</div>
						</div>
						<Separator orientation='horizontal' />
					</div>
					<div className='flex flex-col pt-2 justify-end items-end w-full'>
						<div className='w-full grid grid-cols-2 px-4'>
							<p className='text-lg font-bold'>Estimated total</p>
							<p className='text-lg font-bold ml-auto text-primary'>${calcTotal(subtotal).toFixed(2)}</p>
						</div>
						<div className='p-4 w-full'>
							<Link to={'/basket/checkout'}>
								<Button size={'lg'} className='w-full text-lg'>Checkout</Button>
							</Link>
						</div>
					</div>
				</div>
			</aside>
		</div>
	);
};