import { Separator } from '@/components/ui/separator';
import type { Product, ShoppingBasket } from '@/types';
import { useEffect, useMemo, type FC } from 'react';
import { Button } from '@/components/ui/button';
import { Link, useNavigate } from 'react-router-dom';
import { useShoppingBasket } from '@/hooks/shopping-basket';
import { Stepper } from '@/components/stepper';
import { Trash } from 'lucide-react';
import { ApiImage } from '@/components/api-image';

const PACKING_FEE = 4.51;
const DELIVERY_FEE = 10.99;
const SALES_TAX_PERCENT = 25;

const quantityTotal = (price: number, quantity: number): string => {
	const total = price * quantity;
	return total.toFixed(2);
};

const calcSubtotal = (shoppingBasket: ShoppingBasket): number => {
	return shoppingBasket.reduce((total, item) => {
		return total + item.product.price * item.quantity;
	}, 0);
};

const calcTotal = (price: number): number => {
	const withSalesTax = price * (1 + SALES_TAX_PERCENT / 100);
	return withSalesTax + PACKING_FEE + DELIVERY_FEE;
};

export const ShoppingBasketPage: FC = () => {
	const navigate = useNavigate();
	const { basket } = useShoppingBasket();
	const subtotal: number = useMemo(() => {
		return calcSubtotal(basket);
	}, [basket]);

	useEffect(() => {
		if (basket.length <= 0) {
			void navigate('/');
		}
	}, [basket, navigate]);

	return (
		<div className='w-full flex'>
			<section className='flex flex-col w-3/4'>
				{basket.map((item) => (
					<ShoppingBasketItem key={`shoppingbasket-item-${String(item.product.productId)}`} product={item.product} quantity={item.quantity} />
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

type ShoppingBasketItemProps = {
	product: Product;
	quantity: number;
};

const ShoppingBasketItem: FC<ShoppingBasketItemProps> = ({ product, quantity }) => {
	const { addToBasket, removeFromBasket } = useShoppingBasket();

	return (
		<div>
			<div className='grid grid-cols-6 gap-2 p-4 justify-between items-center'>
				<div className="block aspect-auto h-32 w-32 p-0 relative bg-background">
					<ApiImage
						src={product.imagePath}
						alt={product.productName}
						className='absolute w-full h-full object-contain mix-blend-multiply'
					/>
				</div>
				<p className='text-lg font-bold'>{product.productName}</p>
				<div className='w-max flex flex-col items-center'>
					<p className="text-lg font-bold">Quantity:</p>
					<Stepper
						count={quantity}
						min={0} max={product.stockQuantity}
						minusPressedHandler={() => { removeFromBasket({ product, quantity: 1 }); }}
						plusPressedHandler={() => { addToBasket({ product, quantity: 1 }); }}
					/>
				</div>
				<div className='ml-auto'>
					<p>Price per unit:</p>
					<p className="text-lg font-bold text-green-600 ml-auto">${product.price.toFixed(2)}</p>
				</div>
				<div className='ml-auto'>
					<p>Price combined:</p>
					<p className="text-lg font-bold text-green-600">${quantityTotal(product.price, quantity)}</p>
				</div>
				<Button
					variant={'destructive'}
					size={'icon'}
					className='ml-auto mr-4'
					onClick={() => { removeFromBasket({ product, quantity }); }}
				>
					<Trash />
				</Button>
			</div>
			<Separator orientation='horizontal' className='mt-2' />
		</div>
	);
};